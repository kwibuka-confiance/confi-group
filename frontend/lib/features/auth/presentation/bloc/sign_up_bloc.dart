import 'package:bloc/bloc.dart';
import 'package:equatable/equatable.dart';

import '../../../../core/network/api_exception.dart';
import '../../data/auth_repository.dart';
import '../../data/models/provision_tenant_request.dart';
import '../../data/models/provision_tenant_result.dart';

part 'sign_up_event.dart';
part 'sign_up_state.dart';

/// Drives the sign-up (provision tenant) flow. All the work — calling the API
/// and translating the result into UI state — happens here, keeping the page a
/// pure rendering of [SignUpState].
class SignUpBloc extends Bloc<SignUpEvent, SignUpState> {
  SignUpBloc(this._repository) : super(const SignUpState()) {
    on<SignUpSubmitted>(_onSubmitted);
    on<SignUpReset>((event, emit) => emit(const SignUpState()));
  }

  final AuthRepository _repository;

  Future<void> _onSubmitted(
    SignUpSubmitted event,
    Emitter<SignUpState> emit,
  ) async {
    emit(const SignUpState(status: SignUpStatus.submitting));
    try {
      final result = await _repository.provisionTenant(
        event.request,
        locale: event.locale,
      );
      emit(SignUpState(status: SignUpStatus.success, result: result));
    } on ApiException catch (error) {
      emit(
        SignUpState(
          status: SignUpStatus.failure,
          errorCode: error.code,
          errorMessage: error.message,
          fieldErrors: error.fieldErrors,
        ),
      );
    }
  }
}
