import 'package:bloc/bloc.dart';
import 'package:equatable/equatable.dart';

import '../../../../core/network/api_exception.dart';
import '../../data/auth_repository.dart';
import '../../data/models/session.dart';

part 'login_event.dart';
part 'login_state.dart';

/// Drives sign-in. The page collects the handle, email and password and renders
/// [LoginState]; the API call and its outcome live here.
class LoginBloc extends Bloc<LoginEvent, LoginState> {
  LoginBloc(this._repository) : super(const LoginState()) {
    on<LoginSubmitted>(_onSubmitted);
  }

  final AuthRepository _repository;

  Future<void> _onSubmitted(
    LoginSubmitted event,
    Emitter<LoginState> emit,
  ) async {
    emit(const LoginState(status: LoginStatus.submitting));
    try {
      final session = await _repository.login(
        businessHandle: event.businessHandle,
        email: event.email,
        password: event.password,
        locale: event.locale,
      );
      emit(LoginState(status: LoginStatus.success, session: session));
    } on ApiException catch (error) {
      emit(
        LoginState(
          status: LoginStatus.failure,
          errorCode: error.code,
          errorMessage: error.message,
          fieldErrors: error.fieldErrors,
        ),
      );
    }
  }
}
