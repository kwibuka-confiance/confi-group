import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../core/di/injector.dart';
import '../features/auth/data/auth_repository.dart';
import '../features/auth/presentation/bloc/sign_up_bloc.dart';
import '../features/auth/presentation/pages/sign_up_page.dart';
import '../l10n/app_localizations.dart';
import 'theme.dart';

class ConfiOsApp extends StatelessWidget {
  const ConfiOsApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      onGenerateTitle: (context) => AppLocalizations.of(context).appTitle,
      debugShowCheckedModeBanner: false,
      theme: ConfiTheme.light(),
      darkTheme: ConfiTheme.dark(),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider(
        create: (_) => SignUpBloc(sl<AuthRepository>()),
        child: const SignUpPage(),
      ),
    );
  }
}
