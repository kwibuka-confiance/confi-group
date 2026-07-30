import 'package:flutter/material.dart';

import 'app/app.dart';
import 'core/di/injector.dart';
import 'core/network/api_client.dart';
import 'features/auth/data/session_store.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  configureDependencies();

  // Restore a persisted session before the first frame so a refresh stays signed in.
  final session = await sl<SessionStore>().load();
  if (session != null) {
    sl<ApiClient>().setAuthToken(session.accessToken);
  }

  runApp(ConfiOsApp(initialSession: session));
}
