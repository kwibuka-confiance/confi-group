import 'package:equatable/equatable.dart';

/// Reserved error codes the client reasons about directly. Everything else is
/// surfaced through the server-provided message.
abstract final class ApiErrorCodes {
  static const validationFailed = 'VALIDATION_FAILED';
  static const tenantSlugTaken = 'TENANT_SLUG_TAKEN';
  static const network = 'NETWORK';
  static const unexpected = 'UNEXPECTED';
}

/// A structured error from the ConfiOS backend, mapped from its stable
/// machine-readable envelope: `{ code, message, details: { failures }, traceId }`.
///
/// [fieldErrors] keys are the server's field names (PascalCase, e.g. `Slug`) and
/// values are validation codes (e.g. `REQUIRED`, `INVALID_FORMAT`).
class ApiException extends Equatable implements Exception {
  const ApiException({
    required this.code,
    required this.message,
    this.fieldErrors = const {},
    this.statusCode,
    this.traceId,
  });

  final String code;
  final String message;
  final Map<String, List<String>> fieldErrors;
  final int? statusCode;
  final String? traceId;

  bool get isValidation => code == ApiErrorCodes.validationFailed;

  @override
  List<Object?> get props => [code, message, fieldErrors, statusCode, traceId];

  @override
  String toString() => 'ApiException($code: $message)';
}
