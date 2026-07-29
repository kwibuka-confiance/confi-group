part of 'login_bloc.dart';

enum LoginStatus { initial, submitting, success, failure }

final class LoginState extends Equatable {
  const LoginState({
    this.status = LoginStatus.initial,
    this.session,
    this.errorCode,
    this.errorMessage,
    this.fieldErrors = const {},
  });

  final LoginStatus status;
  final Session? session;
  final String? errorCode;
  final String? errorMessage;
  final Map<String, List<String>> fieldErrors;

  bool get isSubmitting => status == LoginStatus.submitting;

  @override
  List<Object?> get props => [
    status,
    session,
    errorCode,
    errorMessage,
    fieldErrors,
  ];
}
