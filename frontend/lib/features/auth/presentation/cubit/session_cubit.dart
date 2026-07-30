import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_client.dart';
import '../../data/models/session.dart';
import '../../data/session_store.dart';

/// Holds the current [Session] (null when signed out), keeps the API client's
/// bearer token in step, and persists the session so a refresh stays logged in.
class SessionCubit extends Cubit<Session?> {
  SessionCubit(this._apiClient, this._store, {Session? initial}) : super(initial) {
    if (initial != null) {
      _apiClient.setAuthToken(initial.accessToken);
    }
  }

  final ApiClient _apiClient;
  final SessionStore _store;

  Future<void> signIn(Session session) async {
    _apiClient.setAuthToken(session.accessToken);
    emit(session);
    await _store.save(session);
  }

  Future<void> signOut() async {
    _apiClient.setAuthToken(null);
    emit(null);
    await _store.clear();
  }
}
