import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../l10n/app_localizations.dart';
import '../../../auth/data/models/session.dart';
import '../../../auth/presentation/cubit/session_cubit.dart';
import '../../../auth/presentation/widgets/onboarding_widgets.dart';

/// The authenticated landing screen. A starting point: it confirms who is signed
/// in and previews the modules to come.
class HomePage extends StatelessWidget {
  const HomePage({super.key, required this.session});

  final Session session;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Scaffold(
      appBar: AppBar(
        title: Text(session.businessName),
        actions: [
          const ThemeMenuButton(),
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: l10n.signOut,
            onPressed: () => context.read<SessionCubit>().signOut(),
          ),
          const SizedBox(width: 4),
        ],
      ),
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(24),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 620),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Row(
                    children: [
                      CircleAvatar(
                        radius: 28,
                        backgroundColor: scheme.primaryContainer,
                        child: Text(
                          _initials(session.fullName),
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
                              l10n.homeWelcome(session.fullName),
                              style: Theme.of(context).textTheme.headlineSmall
                                  ?.copyWith(fontWeight: FontWeight.w700),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              l10n.homeSignedInTo(session.businessName),
                              style: Theme.of(context).textTheme.bodyMedium
                                  ?.copyWith(color: scheme.onSurfaceVariant),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        children: [
                          ReviewRow(
                            label: l10n.emailFieldLabel,
                            value: session.email,
                          ),
                          ReviewRow(
                            label: l10n.homeBusinessIdLabel,
                            value: session.tenantId,
                          ),
                          ReviewRow(
                            label: l10n.homePermissionsLabel,
                            value: l10n.homePermissionsCount(
                              session.permissions.length,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),
                  Row(
                    children: [
                      _ModuleTile(
                        icon: Icons.inventory_2_outlined,
                        label: l10n.brandPointCatalog,
                      ),
                      const SizedBox(width: 12),
                      _ModuleTile(
                        icon: Icons.point_of_sale_outlined,
                        label: l10n.brandPointSales,
                      ),
                      const SizedBox(width: 12),
                      _ModuleTile(
                        icon: Icons.insights_outlined,
                        label: l10n.brandPointInsights,
                      ),
                    ],
                  ),
                  const SizedBox(height: 20),
                  Text(
                    l10n.homeComingSoon,
                    textAlign: TextAlign.center,
                    style: Theme.of(
                      context,
                    ).textTheme.bodySmall?.copyWith(color: scheme.onSurfaceVariant),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
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

/// A muted, not-yet-active preview of an upcoming module.
class _ModuleTile extends StatelessWidget {
  const _ModuleTile({required this.icon, required this.label});

  final IconData icon;
  final String label;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Expanded(
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 20, horizontal: 12),
        decoration: BoxDecoration(
          color: scheme.surfaceContainerHighest.withValues(alpha: 0.4),
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: scheme.outlineVariant),
        ),
        child: Column(
          children: [
            Icon(icon, color: scheme.onSurfaceVariant),
            const SizedBox(height: 8),
            Text(
              label,
              textAlign: TextAlign.center,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context).textTheme.labelMedium?.copyWith(
                color: scheme.onSurfaceVariant,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
