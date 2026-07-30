import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../features/auth/data/models/session.dart';
import '../features/auth/presentation/cubit/session_cubit.dart';
import '../features/auth/presentation/widgets/onboarding_widgets.dart';
import '../l10n/app_localizations.dart';
import 'theme.dart';

/// A destination in the side navigation.
class NavDestination {
  const NavDestination({required this.label, required this.icon, this.route});

  final String label;
  final IconData icon;

  /// Null for sections that are not built yet; those render disabled.
  final String? route;

  bool get isAvailable => route != null;
}

/// A titled group of destinations.
class NavGroup {
  const NavGroup({required this.title, required this.destinations});

  final String title;
  final List<NavDestination> destinations;
}

/// The signed-in application frame: a dark navigation rail beside a light content
/// panel. Wide layouts show the navigation inline; narrower ones move it into a
/// drawer reached from the top bar.
class AppShell extends StatelessWidget {
  const AppShell({super.key, required this.child, required this.location});

  final Widget child;
  final String location;

  static const double _wideBreakpoint = 1000;
  static const double _navWidth = 264;

  List<NavGroup> _groups(AppLocalizations l10n) => [
    NavGroup(
      title: l10n.navGroupMain,
      destinations: [
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
      ],
    ),
    NavGroup(
      title: l10n.navGroupOperations,
      destinations: [
        NavDestination(label: l10n.navInventory, icon: Icons.warehouse_outlined),
        NavDestination(label: l10n.navSales, icon: Icons.point_of_sale_outlined),
        NavDestination(label: l10n.navReports, icon: Icons.insights_outlined),
      ],
    ),
  ];

  String _titleFor(AppLocalizations l10n) => switch (location) {
    '/products' => l10n.productsTitle,
    _ => l10n.dashboardTitle,
  };

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final session = context.watch<SessionCubit>().state;
    final wide = MediaQuery.sizeOf(context).width >= _wideBreakpoint;

    final nav = Builder(
      builder: (navContext) => _SideNav(
        groups: _groups(l10n),
        location: location,
        session: session,
        onNavigate: (route) {
          if (!wide) {
            Navigator.of(navContext).pop();
          }
          navContext.go(route);
        },
      ),
    );

    final content = Column(
      children: [
        _TopBar(title: _titleFor(l10n), showMenu: !wide, session: session),
        Expanded(child: child),
      ],
    );

    return Scaffold(
      backgroundColor: scheme.surfaceContainerHigh,
      drawer: wide ? null : Drawer(child: nav),
      body: SafeArea(
        child: Padding(
          padding: EdgeInsets.all(wide ? 12 : 0),
          child: Row(
            children: [
              if (wide) ...[
                SizedBox(width: _navWidth, child: nav),
                const SizedBox(width: 12),
              ],
              Expanded(
                child: DecoratedBox(
                  decoration: BoxDecoration(
                    color: scheme.surface,
                    borderRadius: BorderRadius.circular(wide ? 22 : 0),
                    border: wide
                        ? Border.all(color: scheme.outlineVariant)
                        : null,
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(wide ? 22 : 0),
                    child: content,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// The panel header: section title, theme control and the signed-in initials.
class _TopBar extends StatelessWidget {
  const _TopBar({
    required this.title,
    required this.showMenu,
    required this.session,
  });

  final String title;
  final bool showMenu;
  final Session? session;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      padding: const EdgeInsets.fromLTRB(12, 10, 16, 10),
      decoration: BoxDecoration(
        border: Border(bottom: BorderSide(color: scheme.outlineVariant)),
      ),
      child: Row(
        children: [
          if (showMenu)
            IconButton(
              icon: const Icon(Icons.menu),
              tooltip: AppLocalizations.of(context).menuTooltip,
              onPressed: () => Scaffold.of(context).openDrawer(),
            )
          else
            const SizedBox(width: 8),
          Text(
            title,
            style: Theme.of(
              context,
            ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
          ),
          const Spacer(),
          const ThemeMenuButton(),
          const SizedBox(width: 8),
          CircleAvatar(
            radius: 17,
            backgroundColor: scheme.primaryContainer,
            child: Text(
              initialsOf(session?.fullName ?? ''),
              style: TextStyle(
                color: scheme.onPrimaryContainer,
                fontWeight: FontWeight.w700,
                fontSize: 13,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Two-letter initials for an avatar, or "?" when the name is unusable.
String initialsOf(String fullName) {
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

class _SideNav extends StatelessWidget {
  const _SideNav({
    required this.groups,
    required this.location,
    required this.session,
    required this.onNavigate,
  });

  final List<NavGroup> groups;
  final String location;
  final Session? session;
  final ValueChanged<String> onNavigate;

  @override
  Widget build(BuildContext context) {
    final wide = MediaQuery.sizeOf(context).width >= AppShell._wideBreakpoint;

    // The rail keeps its dark treatment in both themes: light content beside a
    // dark rail is the intended contrast, not an artefact of the active theme.
    return DecoratedBox(
      decoration: BoxDecoration(
        color: ConfiTheme.navSurface,
        borderRadius: BorderRadius.circular(wide ? 22 : 0),
      ),
      child: SafeArea(
        child: Column(
          children: [
            const _BrandBlock(),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                children: [
                  for (final group in groups) ...[
                    Padding(
                      padding: const EdgeInsets.fromLTRB(10, 16, 10, 8),
                      child: Text(
                        group.title.toUpperCase(),
                        style: Theme.of(context).textTheme.labelSmall?.copyWith(
                          color: Colors.white.withValues(alpha: 0.45),
                          letterSpacing: 1.1,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                    for (final destination in group.destinations)
                      _NavTile(
                        destination: destination,
                        selected: destination.route == location,
                        onTap: destination.isAvailable
                            ? () => onNavigate(destination.route!)
                            : null,
                      ),
                  ],
                ],
              ),
            ),
            _UserCard(session: session),
          ],
        ),
      ),
    );
  }
}

class _BrandBlock extends StatelessWidget {
  const _BrandBlock();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 22, 20, 6),
      child: Row(
        children: [
          Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
              color: ConfiTheme.seed,
              borderRadius: BorderRadius.circular(10),
            ),
            child: const Icon(
              Icons.storefront_rounded,
              color: Colors.white,
              size: 20,
            ),
          ),
          const SizedBox(width: 12),
          Text(
            l10n.appTitle,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w700,
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
    final disabled = onTap == null;
    final foreground = selected
        ? Colors.white
        : Colors.white.withValues(alpha: disabled ? 0.34 : 0.72);

    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Material(
        color: selected ? ConfiTheme.seed : Colors.transparent,
        borderRadius: BorderRadius.circular(11),
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(11),
          hoverColor: Colors.white.withValues(alpha: 0.06),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
            child: Row(
              children: [
                Icon(destination.icon, size: 20, color: foreground),
                const SizedBox(width: 12),
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
                    padding: const EdgeInsets.symmetric(
                      horizontal: 7,
                      vertical: 3,
                    ),
                    decoration: BoxDecoration(
                      color: Colors.white.withValues(alpha: 0.08),
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: Text(
                      l10n.comingSoonBadge,
                      style: Theme.of(context).textTheme.labelSmall?.copyWith(
                        color: Colors.white.withValues(alpha: 0.5),
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

class _UserCard extends StatelessWidget {
  const _UserCard({required this.session});

  final Session? session;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Padding(
      padding: const EdgeInsets.all(12),
      child: Container(
        padding: const EdgeInsets.fromLTRB(12, 10, 6, 10),
        decoration: BoxDecoration(
          color: Colors.white.withValues(alpha: 0.06),
          borderRadius: BorderRadius.circular(14),
        ),
        child: Row(
          children: [
            CircleAvatar(
              radius: 17,
              backgroundColor: ConfiTheme.seed,
              child: Text(
                initialsOf(session?.fullName ?? ''),
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
                  fontSize: 13,
                ),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    session?.fullName ?? '',
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: Colors.white,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  Text(
                    session?.email ?? '',
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: Theme.of(context).textTheme.labelSmall?.copyWith(
                      color: Colors.white.withValues(alpha: 0.55),
                    ),
                  ),
                ],
              ),
            ),
            IconButton(
              icon: const Icon(Icons.logout, size: 18),
              color: Colors.white.withValues(alpha: 0.7),
              tooltip: l10n.signOut,
              onPressed: () => context.read<SessionCubit>().signOut(),
            ),
          ],
        ),
      ),
    );
  }
}
