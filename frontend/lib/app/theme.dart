import 'package:flutter/material.dart';

/// Material 3 theme built from a single brand seed, tuned for both light and
/// dark. Typography pairs Sora (headings) with Inter (body/UI), both bundled.
abstract final class ConfiTheme {
  /// Brand seed (placeholder). Swap for the official brand colour when defined.
  static const Color seed = Color(0xFF12715E);

  static const String _headingFont = 'Sora';
  static const String _bodyFont = 'Inter';

  static ThemeData light() => _build(Brightness.light);

  static ThemeData dark() => _build(Brightness.dark);

  static ThemeData _build(Brightness brightness) {
    final scheme = ColorScheme.fromSeed(
      seedColor: seed,
      brightness: brightness,
    );
    final isDark = brightness == Brightness.dark;
    final base = ThemeData(
      useMaterial3: true,
      colorScheme: scheme,
      fontFamily: _bodyFont,
    );
    final fieldRadius = BorderRadius.circular(14);

    return base.copyWith(
      scaffoldBackgroundColor: scheme.surface,
      textTheme: _textTheme(base.textTheme),
      appBarTheme: AppBarTheme(
        backgroundColor: scheme.surface,
        foregroundColor: scheme.onSurface,
        centerTitle: false,
        elevation: 0,
        scrolledUnderElevation: 0.5,
        titleTextStyle: TextStyle(
          fontFamily: _headingFont,
          fontSize: 20,
          fontWeight: FontWeight.w700,
          color: scheme.onSurface,
        ),
      ),
      cardTheme: CardThemeData(
        elevation: 0,
        color: scheme.surface,
        surfaceTintColor: Colors.transparent,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(24),
          side: BorderSide(color: scheme.outlineVariant),
        ),
        clipBehavior: Clip.antiAlias,
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: scheme.surfaceContainerHighest.withValues(
          alpha: isDark ? 0.35 : 0.6,
        ),
        border: OutlineInputBorder(
          borderRadius: fieldRadius,
          borderSide: BorderSide(color: scheme.outlineVariant),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: fieldRadius,
          borderSide: BorderSide(color: scheme.outlineVariant),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: fieldRadius,
          borderSide: BorderSide(color: scheme.primary, width: 2),
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 16,
          vertical: 18,
        ),
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          // Height only. Size.fromHeight would force an infinite minimum width,
          // which breaks any button placed in a Row; parents stretch buttons.
          minimumSize: const Size(64, 52),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
          textStyle: const TextStyle(
            fontFamily: _bodyFont,
            fontSize: 16,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          minimumSize: const Size(64, 52),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
          textStyle: const TextStyle(
            fontFamily: _bodyFont,
            fontSize: 16,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }

  /// Headings in Sora, everything else in Inter. Display/headline weights are
  /// bumped so titles feel deliberate rather than default.
  static TextTheme _textTheme(TextTheme base) {
    TextStyle? heading(TextStyle? style) =>
        style?.copyWith(fontFamily: _headingFont, fontWeight: FontWeight.w700);
    TextStyle? body(TextStyle? style) =>
        style?.copyWith(fontFamily: _bodyFont);

    return base.copyWith(
      displayLarge: heading(base.displayLarge),
      displayMedium: heading(base.displayMedium),
      displaySmall: heading(base.displaySmall),
      headlineLarge: heading(base.headlineLarge),
      headlineMedium: heading(base.headlineMedium),
      headlineSmall: heading(base.headlineSmall),
      titleLarge: heading(base.titleLarge),
      titleMedium: body(base.titleMedium),
      titleSmall: body(base.titleSmall),
      bodyLarge: body(base.bodyLarge),
      bodyMedium: body(base.bodyMedium),
      bodySmall: body(base.bodySmall),
      labelLarge: body(base.labelLarge),
      labelMedium: body(base.labelMedium),
      labelSmall: body(base.labelSmall),
    );
  }
}
