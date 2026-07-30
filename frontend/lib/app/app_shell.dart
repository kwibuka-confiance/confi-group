import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../features/auth/data/models/session.dart';
import '../features/auth/presentation/cubit/session_cubit.dart';
import '../features/auth/presentation/widgets/onboarding_widgets.dart';
import '../l10n/app_localizations.dart';

/// A destination in the side navigation.
class NavDestination {
  const NavDestination({
    required this.label,
    required this.icon,
    this.route,
  });

  final String label;
  final IconData icon;

  /// Null for sections that are not built yet; those render disabled.
  final String? route;

  bool get isAvailable => route != null;
}

/// The signed-in application frame: a persistent side navigation next to the
/// current section. Wide layouts show the navigation inline; narrower ones move
/// it into a drawer reached from the app bar.
class AppShell extends StatelessWidget {
  const AppShell({super.key, required this.child, required this.location});

  final Widget child;
  final String location;

  static const double _wideBreakpoint = 1000;
  static const double _navWidth = 268;

  List<NavDestination> _destinations(AppLocalizations l10n) => [
    NavDestination(
      label: l10n.navDashboard,
      icon: Icons.space_dashboard_outlined,
      route: '/dashboard',
    ),
    NavDestination(
      label: l10n.navProducts,
      icon: Icons.inventory_2_outlined,
      route: '/products',
    ),
    NavDestination(label: l10n.navInventory, icon: Icons.warehouse_outlined),
    NavDestination(label: l10n.navSales, icon: Icons.point_of_sale_outlined),
    NavDestination(label: l10n.navReports, icon: Icons.insights_outlined),
  ];

  String _titleFor(AppLocalizations l10n) => switch (location) {
    '/products' => l10n.productsTitle,
    _ => l10n.dashboardTitle,
  };

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final session = context.watch<SessionCubit>().state;
    final destinations = _destinations(l10n);
    final wide = MediaQuery.sizeOf(context).width >= _wideBreakpoint;

    final nav = _SideNav(
      destinations: destinations,
      location: location,
      session: session,
      onNavigate: (route) {
        if (!wide) {
          Navigator.of(context).pop();
        }
        context.go(route);
      },
    );

    return Scaffold(
      drawer: wide ? null : Drawer(child: nav),
      appBar: AppBar(
        title: Text(_titleFor(l10n)),
        actions: const [ThemeMenuButton(), SizedBox(width: 4)],
      ),
      body: wide
          ? Row(
              children: [
                SizedBox(width: _navWidth, child: nav),
                const VerticalDivider(width: 1, thickness: 1),
                Expanded(child: child),
              ],
            )
          : child,
    );
  }
}

class _SideNav extends StatelessWidget {
  const _SideNav({
    required this.destinations,
    required this.location,
    required this.session,
    required this.onNavigate,
  });

  final List<NavDestination> destinations;
  final String location;
  final Session? session;
  final ValueChanged<String> onNavigate;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Material(
      color: scheme.surfaceContainerLow,
      child: SafeArea(
        child: Column(
          children: [
            _BusinessHeader(session: session),
            const Divider(height: 1),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
                children: [
                  for (final destination in destinations)
                    _NavTile(
                      destination: destination,
                      selected: destination.route == location,
                      onTap: destination.isAvailable
                          ? () => onNavigate(destination.route!)
                          : null,
                    ),
                ],
              ),
            ),
            const Divider(height: 1),
            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  if (session != null)
                    Padding(
                      padding: const EdgeInsets.only(bottom: 8, left: 4),
                      child: Text(
                        session!.email,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: scheme.onSurfaceVariant,
                        ),
                      ),
                    ),
                  OutlinedButton.icon(
                    onPressed: () => context.read<SessionCubit>().signOut(),
                    icon: const Icon(Icons.logout, size: 18),
                    label: Text(l10n.signOut),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _BusinessHeader extends StatelessWidget {
  const _BusinessHeader({required this.session});

  final Session? session;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 20, 16, 20),
      child: Row(
        children: [
          Container(
            width: 42,
            height: 42,
            decoration: BoxDecoration(
              color: scheme.primary,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(
              Icons.storefront_rounded,
              color: scheme.onPrimary,
              size: 22,
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  session?.businessName ?? l10n.appTitle,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
                Text(
                  l10n.appTitle,
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    color: scheme.onSurfaceVariant,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _NavTile extends StatelessWidget {
  const _NavTile({
    required this.destination,
    required this.selected,
    this.onTap,
  });

  final NavDestination destination;
  final bool selected;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final disabled = onTap == null;

    final foreground = selected
        ? scheme.onSecondaryContainer
        : disabled
        ? scheme.onSurfaceVariant.withValues(alpha: 0.5)
        : scheme.onSurfaceVariant;

    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Material(
        color: selected ? scheme.secondaryContainer : Colors.transparent,
        borderRadius: BorderRadius.circular(12),
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(12),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 13),
            child: Row(
              children: [
                Icon(destination.icon, size: 21, color: foreground),
                const SizedBox(width: 14),
                Expanded(
                  child: Text(
                    destination.label,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: foreground,
                      fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
                    ),
                  ),
                ),
                // Sections that are not built yet say so, rather than looking broken.
                if (disabled)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                    decoration: BoxDecoration(
                      color: scheme.surfaceContainerHighest,
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: Text(
                      l10n.comingSoonBadge,
                      style: Theme.of(context).textTheme.labelSmall?.copyWith(
                        color: scheme.onSurfaceVariant,
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
