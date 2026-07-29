import 'package:flutter/foundation.dart';

/// Static configuration resolved at startup.
///
/// The API base URL can be overridden at build time with
/// `--dart-define=API_BASE_URL=...`. Otherwise it is chosen per platform: the
/// Android emulator reaches the host machine through 10.0.2.2, while web and the
/// desktop/iOS simulators use localhost directly.
abstract final class AppConfig {
  static const _override = String.fromEnvironment('API_BASE_URL');

  static String get apiBaseUrl {
    if (_override.isNotEmpty) {
      return _override;
    }
    if (kIsWeb) {
      return 'http://localhost:5080';
    }
    if (defaultTargetPlatform == TargetPlatform.android) {
      return 'http://10.0.2.2:5080';
    }
    return 'http://localhost:5080';
  }
}
