import 'package:flutter/material.dart';

/// Material 3 theme built from a single brand seed colour, in light and dark.
abstract final class ConfiTheme {
  /// Brand seed (placeholder). Swap for the official brand colour when defined.
  static const Color _seed = Color(0xFF00695C);

  static ThemeData light() => _base(Brightness.light);

  static ThemeData dark() => _base(Brightness.dark);

  static ThemeData _base(Brightness brightness) {
    return ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: _seed,
        brightness: brightness,
      ),
      inputDecorationTheme: const InputDecorationTheme(
        border: OutlineInputBorder(),
      ),
    );
  }
}
