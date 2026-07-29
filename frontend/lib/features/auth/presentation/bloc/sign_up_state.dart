part of 'sign_up_bloc.dart';

enum SignUpStatus { initial, submitting, success, failure }

final class SignUpState extends Equatable {
  const SignUpState({
    this.status = SignUpStatus.initial,
    this.result,
    this.errorCode,
    this.errorMessage,
    this.fieldErrors = const {},
  });

  final SignUpStatus status;
  final ProvisionTenantResult? result;

  /// Top-level error code (e.g. `TENANT_SLUG_TAKEN`), when the failure is not a
  /// per-field validation error.
  final String? errorCode;

  /// Server-provided message, used as a fallback when the code is not one the
  /// client maps to its own localised string.
  final String? errorMessage;

  /// Per-field validation codes keyed by the server's field names (PascalCase).
  final Map<String, List<String>> fieldErrors;

  bool get isSubmitting => status == SignUpStatus.submitting;

  @override
  List<Object?> get props => [
    status,
    result,
    errorCode,
    errorMessage,
    fieldErrors,
  ];
}
