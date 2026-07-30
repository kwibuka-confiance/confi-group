import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';

import 'models/session.dart';

/// Persists the signed-in [Session] so a page refresh (on web) or a restart keeps
/// the user logged in. An expired session is discarded on load.
///
/// The token is stored in the platform's key/value store (localStorage on web).
/// That is standard for this kind of app; hardening to a more secure store is a
/// later step.
class SessionStore {
  const SessionStore();

  static const _key = 'confios.session';

  Future<void> save(Session session) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_key, jsonEncode(session.toJson()));
  }

  Future<Session?> load() async {
    // Everything is guarded: a corrupt or legacy stored value must degrade to
    // "signed out", never throw and blank the app during start-up.
    try {
      final prefs = await SharedPreferences.getInstance();
      final raw = prefs.getString(_key);
      if (raw == null) {
        return null;
      }
      final session = Session.fromJson(jsonDecode(raw) as Map<String, dynamic>);
      if (session.isExpired || session.accessToken.isEmpty) {
        await clear();
        return null;
      }
      return session;
    } on Object {
      return null;
    }
  }

  Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_key);
  }
}
