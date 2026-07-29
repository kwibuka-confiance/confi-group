import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../core/di/injector.dart';
import '../core/network/api_client.dart';
import '../features/auth/data/models/session.dart';
import '../features/auth/presentation/cubit/session_cubit.dart';
import '../features/auth/presentation/pages/auth_gate.dart';
import '../features/home/presentation/pages/home_page.dart';
import '../l10n/app_localizations.dart';
import 'theme.dart';
import 'theme_cubit.dart';

class ConfiOsApp extends StatelessWidget {
  const ConfiOsApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => ThemeCubit()),
        BlocProvider(create: (_) => SessionCubit(sl<ApiClient>())),
      ],
      child: BlocBuilder<ThemeCubit, ThemeMode>(
        builder: (context, themeMode) {
          return MaterialApp(
            onGenerateTitle: (context) => AppLocalizations.of(context).appTitle,
            debugShowCheckedModeBanner: false,
            themeMode: themeMode,
            theme: ConfiTheme.light(),
            darkTheme: ConfiTheme.dark(),
            localizationsDelegates: AppLocalizations.localizationsDelegates,
            supportedLocales: AppLocalizations.supportedLocales,
            home: BlocBuilder<SessionCubit, Session?>(
              builder: (context, session) => session == null
                  ? const AuthGate()
                  : HomePage(session: session),
            ),
          );
        },
      ),
    );
  }
}
