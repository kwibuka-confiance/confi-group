part of 'sign_up_bloc.dart';

sealed class SignUpEvent extends Equatable {
  const SignUpEvent();

  @override
  List<Object?> get props => [];
}

/// Submit the assembled sign-up form. [locale] is forwarded as Accept-Language
/// so the backend localises any error it returns.
final class SignUpSubmitted extends SignUpEvent {
  const SignUpSubmitted(this.request, {this.locale});

  final ProvisionTenantRequest request;
  final String? locale;

  @override
  List<Object?> get props => [request, locale];
}

/// Clear the result and return to an empty form.
final class SignUpReset extends SignUpEvent {
  const SignUpReset();
}
