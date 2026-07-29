import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/di/injector.dart';
import '../../data/auth_repository.dart';
import '../bloc/login_bloc.dart';
import '../bloc/sign_up_bloc.dart';
import 'login_page.dart';
import 'sign_up_page.dart';

enum _AuthScreen { signIn, signUp }

/// Shown while signed out: switches between sign-in and the sign-up wizard, and
/// carries the new business handle from sign-up straight into the sign-in form.
class AuthGate extends StatefulWidget {
  const AuthGate({super.key});

  @override
  State<AuthGate> createState() => _AuthGateState();
}

class _AuthGateState extends State<AuthGate> {
  _AuthScreen _screen = _AuthScreen.signIn;
  String? _prefillHandle;

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => LoginBloc(sl<AuthRepository>())),
        BlocProvider(create: (_) => SignUpBloc(sl<AuthRepository>())),
      ],
      child: _screen == _AuthScreen.signIn
          ? LoginPage(
              initialHandle: _prefillHandle,
              onCreateBusiness: () =>
                  setState(() => _screen = _AuthScreen.signUp),
            )
          : SignUpPage(
              onSignIn: (handle) => setState(() {
                _prefillHandle = handle;
                _screen = _AuthScreen.signIn;
              }),
            ),
    );
  }
}
