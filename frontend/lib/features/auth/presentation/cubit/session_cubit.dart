import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_client.dart';
import '../../data/models/session.dart';

/// Holds the current [Session] (null when signed out) and keeps the API client's
/// bearer token in step, so every authenticated call carries it automatically.
class SessionCubit extends Cubit<Session?> {
  SessionCubit(this._apiClient) : super(null);

  final ApiClient _apiClient;

  void signIn(Session session) {
    _apiClient.setAuthToken(session.accessToken);
    emit(session);
  }

  void signOut() {
    _apiClient.setAuthToken(null);
    emit(null);
  }
}
