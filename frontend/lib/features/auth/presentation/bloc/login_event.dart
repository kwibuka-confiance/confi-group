part of 'login_bloc.dart';

sealed class LoginEvent extends Equatable {
  const LoginEvent();

  @override
  List<Object?> get props => [];
}

/// Submit the sign-in form. [locale] is forwarded as Accept-Language so the
/// backend localises any error.
final class LoginSubmitted extends LoginEvent {
  const LoginSubmitted({
    required this.businessHandle,
    required this.email,
    required this.password,
    this.locale,
  });

  final String businessHandle;
  final String email;
  final String password;
  final String? locale;

  @override
  List<Object?> get props => [businessHandle, email, password, locale];
}
