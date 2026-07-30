import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../core/di/injector.dart';
import '../core/network/api_client.dart';
import '../core/routing/go_router_refresh_stream.dart';
import '../features/auth/data/auth_repository.dart';
import '../features/auth/data/models/session.dart';
import '../features/auth/data/session_store.dart';
import '../features/auth/presentation/bloc/login_bloc.dart';
import '../features/auth/presentation/bloc/sign_up_bloc.dart';
import '../features/auth/presentation/cubit/session_cubit.dart';
import '../features/auth/presentation/pages/login_page.dart';
import '../features/auth/presentation/pages/sign_up_page.dart';
import '../features/catalog/presentation/pages/products_page.dart';
import '../features/home/presentation/pages/home_page.dart';
import '../l10n/app_localizations.dart';
import 'theme.dart';
import 'theme_cubit.dart';

class ConfiOsApp extends StatefulWidget {
  const ConfiOsApp({super.key, this.initialSession});

  final Session? initialSession;

  @override
  State<ConfiOsApp> createState() => _ConfiOsAppState();
}

class _ConfiOsAppState extends State<ConfiOsApp> {
  late final ThemeCubit _theme = ThemeCubit();
  late final SessionCubit _session = SessionCubit(
    sl<ApiClient>(),
    sl<SessionStore>(),
    initial: widget.initialSession,
  );
  late final GoRouter _router = _buildRouter();

  GoRouter _buildRouter() {
    return GoRouter(
      initialLocation: '/signin',
      refreshListenable: GoRouterRefreshStream(_session.stream),
      errorBuilder: (context, state) => Scaffold(
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Text('Route error: ${state.error}'),
          ),
        ),
      ),
      redirect: (context, state) {
        final loggedIn = _session.state != null;
        final location = state.matchedLocation;
        final onAuthScreen = location == '/signin' || location == '/signup';
        if (!loggedIn && !onAuthScreen) {
          return '/signin';
        }
        if (loggedIn && onAuthScreen) {
          return '/home';
        }
        return null;
      },
      routes: [
        GoRoute(
          path: '/signin',
          builder: (context, state) => BlocProvider(
            create: (_) => LoginBloc(sl<AuthRepository>()),
            child: LoginPage(
              initialHandle: state.uri.queryParameters['handle'],
              onCreateBusiness: () => context.go('/signup'),
            ),
          ),
        ),
        GoRoute(
          path: '/signup',
          builder: (context, state) => BlocProvider(
            create: (_) => SignUpBloc(sl<AuthRepository>()),
            child: SignUpPage(
              onSignIn: (handle) => context.go('/signin?handle=$handle'),
            ),
          ),
        ),
        GoRoute(
          path: '/home',
          builder: (context, state) {
            final session = _session.state;
            return session == null
                ? const SizedBox.shrink()
                : HomePage(session: session);
          },
        ),
        GoRoute(
          path: '/products',
          builder: (context, state) => const ProductsPage(),
        ),
      ],
    );
  }

  @override
  void dispose() {
    _router.dispose();
    _session.close();
    _theme.close();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider.value(value: _theme),
        BlocProvider.value(value: _session),
      ],
      child: BlocBuilder<ThemeCubit, ThemeMode>(
        builder: (context, themeMode) {
          return MaterialApp.router(
            onGenerateTitle: (context) => AppLocalizations.of(context).appTitle,
            debugShowCheckedModeBanner: false,
            themeMode: themeMode,
            theme: ConfiTheme.light(),
            darkTheme: ConfiTheme.dark(),
            localizationsDelegates: AppLocalizations.localizationsDelegates,
            supportedLocales: AppLocalizations.supportedLocales,
            routerConfig: _router,
          );
        },
      ),
    );
  }
}
