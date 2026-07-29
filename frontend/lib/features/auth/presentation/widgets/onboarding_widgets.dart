import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../app/theme.dart';
import '../../../../app/theme_cubit.dart';
import '../../../../l10n/app_localizations.dart';

/// The marketing/brand side shown next to the form on wide screens.
class BrandPanel extends StatelessWidget {
  const BrandPanel({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    const onBrand = Colors.white;

    // A fixed deep-teal gradient in both themes: white-on-teal reads as a
    // premium brand surface, instead of a washed-out light panel in dark mode.
    return DecoratedBox(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            ConfiTheme.seed,
            Color.lerp(ConfiTheme.seed, Colors.black, 0.55)!,
          ],
        ),
      ),
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Row(
              children: [
                Icon(Icons.storefront_rounded, color: onBrand, size: 32),
                const SizedBox(width: 12),
                Text(
                  l10n.appTitle,
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    color: onBrand,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 40),
            Text(
              l10n.brandTagline,
              style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                color: onBrand,
                fontWeight: FontWeight.w700,
                height: 1.15,
              ),
            ),
            const SizedBox(height: 16),
            Text(
              l10n.brandBlurb,
              style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                color: onBrand.withValues(alpha: 0.9),
                height: 1.4,
              ),
            ),
            const SizedBox(height: 40),
            _point(context, Icons.inventory_2_outlined, l10n.brandPointCatalog),
            _point(context, Icons.point_of_sale_outlined, l10n.brandPointSales),
            _point(context, Icons.insights_outlined, l10n.brandPointInsights),
          ],
        ),
      ),
    );
  }

  Widget _point(BuildContext context, IconData icon, String text) {
    const onBrand = Colors.white;
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: Row(
        children: [
          Icon(icon, color: onBrand, size: 22),
          const SizedBox(width: 14),
          Expanded(
            child: Text(
              text,
              style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                color: onBrand.withValues(alpha: 0.95),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// A segmented progress bar with a short label under each step.
class WizardProgress extends StatelessWidget {
  const WizardProgress({
    super.key,
    required this.currentStep,
    required this.labels,
  });

  final int currentStep;
  final List<String> labels;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Row(
      children: List.generate(labels.length, (index) {
        final reached = index <= currentStep;
        return Expanded(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 3),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  height: 5,
                  decoration: BoxDecoration(
                    color: reached ? scheme.primary : scheme.surfaceContainerHighest,
                    borderRadius: BorderRadius.circular(4),
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  labels[index],
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    color: reached ? scheme.primary : scheme.onSurfaceVariant,
                    fontWeight: reached ? FontWeight.w600 : FontWeight.w500,
                  ),
                ),
              ],
            ),
          ),
        );
      }),
    );
  }
}

/// App-bar action to switch between system / light / dark themes.
class ThemeMenuButton extends StatelessWidget {
  const ThemeMenuButton({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final mode = context.watch<ThemeCubit>().state;

    return PopupMenuButton<ThemeMode>(
      tooltip: l10n.themeTooltip,
      icon: Icon(switch (mode) {
        ThemeMode.system => Icons.brightness_auto_outlined,
        ThemeMode.light => Icons.light_mode_outlined,
        ThemeMode.dark => Icons.dark_mode_outlined,
      }),
      onSelected: (selected) => context.read<ThemeCubit>().select(selected),
      itemBuilder: (context) => [
        _item(ThemeMode.system, Icons.brightness_auto_outlined, l10n.themeSystem, mode),
        _item(ThemeMode.light, Icons.light_mode_outlined, l10n.themeLight, mode),
        _item(ThemeMode.dark, Icons.dark_mode_outlined, l10n.themeDark, mode),
      ],
    );
  }

  PopupMenuItem<ThemeMode> _item(
    ThemeMode value,
    IconData icon,
    String label,
    ThemeMode current,
  ) {
    return PopupMenuItem(
      value: value,
      child: Row(
        children: [
          Icon(icon, size: 20),
          const SizedBox(width: 12),
          Text(label),
          if (value == current) ...[
            const Spacer(),
            const Icon(Icons.check, size: 18),
          ],
        ],
      ),
    );
  }
}

/// An inline error banner for top-level failures (network, handle taken).
class InfoBanner extends StatelessWidget {
  const InfoBanner({super.key, required this.message});

  final String message;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: scheme.errorContainer,
        borderRadius: BorderRadius.circular(14),
      ),
      child: Row(
        children: [
          Icon(Icons.error_outline, color: scheme.onErrorContainer),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              message,
              style: TextStyle(color: scheme.onErrorContainer),
            ),
          ),
        ],
      ),
    );
  }
}

/// A label/value row used in the review step.
class ReviewRow extends StatelessWidget {
  const ReviewRow({super.key, required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 130,
            child: Text(
              label,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: scheme.onSurfaceVariant,
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              value,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
