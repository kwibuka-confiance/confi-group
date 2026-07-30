import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../core/di/injector.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../auth/data/models/session.dart';
import '../../../auth/presentation/cubit/session_cubit.dart';
import '../../../catalog/data/models/product.dart';
import '../../../catalog/data/product_repository.dart';
import '../cubit/dashboard_cubit.dart';

/// The signed-in landing view: a row of figures drawn from the catalog, the most
/// recently added products, and the actions worth reaching from here.
class DashboardPage extends StatelessWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    final locale = Localizations.localeOf(context).languageCode;
    return BlocProvider(
      create: (_) => DashboardCubit(sl<ProductRepository>())..load(locale),
      child: _DashboardView(locale: locale),
    );
  }
}

class _DashboardView extends StatelessWidget {
  const _DashboardView({required this.locale});

  final String locale;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final session = context.watch<SessionCubit>().state;

    return BlocBuilder<DashboardCubit, DashboardState>(
      builder: (context, state) {
        return RefreshIndicator(
          onRefresh: () => context.read<DashboardCubit>().load(locale),
          child: ListView(
            padding: const EdgeInsets.all(24),
            children: [
              _Greeting(session: session),
              const SizedBox(height: 24),
              if (state.status == DashboardStatus.error)
                _ErrorNotice(
                  onRetry: () => context.read<DashboardCubit>().load(locale),
                )
              else ...[
                _StatGrid(state: state, session: session, locale: locale),
                const SizedBox(height: 28),
                _RecentProducts(state: state, locale: locale),
              ],
              const SizedBox(height: 28),
              Text(
                l10n.quickActionsTitle,
                style: Theme.of(
                  context,
                ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
              ),
              const SizedBox(height: 12),
              Wrap(
                spacing: 12,
                runSpacing: 12,
                children: [
                  FilledButton.tonalIcon(
                    onPressed: () => context.go('/products'),
                    icon: const Icon(Icons.inventory_2_outlined),
                    label: Text(l10n.navProducts),
                  ),
                ],
              ),
            ],
          ),
        );
      },
    );
  }
}

class _Greeting extends StatelessWidget {
  const _Greeting({required this.session});

  final Session? session;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final name = session?.fullName ?? '';

    return Row(
      children: [
        CircleAvatar(
          radius: 26,
          backgroundColor: scheme.primaryContainer,
          child: Text(
            _initials(name),
            style: TextStyle(
              color: scheme.onPrimaryContainer,
              fontWeight: FontWeight.w700,
            ),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                l10n.homeWelcome(name),
                style: Theme.of(
                  context,
                ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
              ),
              const SizedBox(height: 2),
              Text(
                l10n.homeSignedInTo(session?.businessName ?? ''),
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: scheme.onSurfaceVariant,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  static String _initials(String fullName) {
    final parts = fullName.trim().split(RegExp(r'\s+'));
    if (parts.isEmpty || parts.first.isEmpty) {
      return '?';
    }
    if (parts.length == 1) {
      return parts.first.characters.first.toUpperCase();
    }
    return (parts.first.characters.first + parts.last.characters.first)
        .toUpperCase();
  }
}

/// The figures row. Tiles reflow by available width rather than a fixed column
/// count, so the same layout works on a phone and a desktop.
class _StatGrid extends StatelessWidget {
  const _StatGrid({
    required this.state,
    required this.session,
    required this.locale,
  });

  final DashboardState state;
  final Session? session;
  final String locale;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final loading = state.status == DashboardStatus.loading;

    return LayoutBuilder(
      builder: (context, constraints) {
        const spacing = 12.0;
        final columns = switch (constraints.maxWidth) {
          >= 900 => 4,
          >= 560 => 2,
          _ => 1,
        };
        final tileWidth =
            (constraints.maxWidth - spacing * (columns - 1)) / columns;

        final tiles = <Widget>[
          _StatTile(
            label: l10n.statProducts,
            value: loading ? null : '${state.totalProducts}',
            icon: Icons.inventory_2_outlined,
          ),
          _StatTile(
            label: l10n.statActiveProducts,
            value: loading ? null : '${state.activeProducts}',
            icon: Icons.check_circle_outline,
          ),
          _StatTile(
            label: l10n.statCatalogValue,
            value: loading ? null : _catalogValue(l10n),
            icon: Icons.payments_outlined,
          ),
          _StatTile(
            label: l10n.statPermissions,
            value: '${session?.permissions.length ?? 0}',
            icon: Icons.verified_user_outlined,
          ),
        ];

        return Wrap(
          spacing: spacing,
          runSpacing: spacing,
          children: [
            for (final tile in tiles) SizedBox(width: tileWidth, child: tile),
          ],
        );
      },
    );
  }

  /// Amounts in different currencies are never summed. With one currency the
  /// total is shown; with several, the count of currencies is reported instead.
  String _catalogValue(AppLocalizations l10n) {
    final values = state.valueByCurrency;
    if (values.isEmpty) {
      return '—';
    }
    if (values.length > 1) {
      return l10n.currencyCount(values.length);
    }
    final entry = values.entries.first;
    final amount = NumberFormat.decimalPattern(locale).format(entry.value);
    return '$amount ${entry.key}';
  }
}

class _StatTile extends StatelessWidget {
  const _StatTile({required this.label, required this.value, required this.icon});

  final String label;

  /// Null while loading, so the tile shows a placeholder instead of a wrong zero.
  final String? value;
  final IconData icon;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: scheme.surfaceContainerLow,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: scheme.outlineVariant),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, size: 18, color: scheme.primary),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  label,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.labelMedium?.copyWith(
                    color: scheme.onSurfaceVariant,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          value == null
              ? SizedBox(
                  height: 30,
                  child: Align(
                    alignment: Alignment.centerLeft,
                    child: SizedBox(
                      width: 56,
                      height: 10,
                      child: DecoratedBox(
                        decoration: BoxDecoration(
                          color: scheme.surfaceContainerHighest,
                          borderRadius: BorderRadius.circular(4),
                        ),
                      ),
                    ),
                  ),
                )
              : Text(
                  value!,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
        ],
      ),
    );
  }
}

class _RecentProducts extends StatelessWidget {
  const _RecentProducts({required this.state, required this.locale});

  final DashboardState state;
  final String locale;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                l10n.recentlyAddedTitle,
                style: Theme.of(
                  context,
                ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
              ),
            ),
            if (state.recentProducts.isNotEmpty)
              TextButton(
                onPressed: () => context.go('/products'),
                child: Text(l10n.viewAllAction),
              ),
          ],
        ),
        const SizedBox(height: 8),
        if (state.status == DashboardStatus.loaded &&
            state.recentProducts.isEmpty)
          Container(
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              color: scheme.surfaceContainerLow,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: scheme.outlineVariant),
            ),
            child: Text(
              l10n.dashboardEmptyHint,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: scheme.onSurfaceVariant,
              ),
            ),
          )
        else
          Container(
            decoration: BoxDecoration(
              color: scheme.surfaceContainerLow,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: scheme.outlineVariant),
            ),
            child: Column(
              children: [
                for (var i = 0; i < state.recentProducts.length; i++) ...[
                  if (i > 0) Divider(height: 1, color: scheme.outlineVariant),
                  _ProductRow(product: state.recentProducts[i], locale: locale),
                ],
              ],
            ),
          ),
      ],
    );
  }
}

class _ProductRow extends StatelessWidget {
  const _ProductRow({required this.product, required this.locale});

  final Product product;
  final String locale;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final amount = NumberFormat.decimalPattern(locale).format(product.priceAmount);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Row(
        children: [
          Icon(Icons.inventory_2_outlined, size: 20, color: scheme.primary),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  product.name,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
                Text(
                  product.sku,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: scheme.onSurfaceVariant,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 12),
          Text(
            '$amount ${product.currencyCode}',
            style: Theme.of(
              context,
            ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w700),
          ),
        ],
      ),
    );
  }
}

class _ErrorNotice extends StatelessWidget {
  const _ErrorNotice({required this.onRetry});

  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: scheme.errorContainer,
        borderRadius: BorderRadius.circular(18),
      ),
      child: Row(
        children: [
          Icon(Icons.error_outline, color: scheme.onErrorContainer),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              l10n.productsLoadError,
              style: TextStyle(color: scheme.onErrorContainer),
            ),
          ),
          TextButton(onPressed: onRetry, child: Text(l10n.retryButton)),
        ],
      ),
    );
  }
}
