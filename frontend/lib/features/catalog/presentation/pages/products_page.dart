import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';

import '../../../../core/di/injector.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../auth/presentation/widgets/onboarding_widgets.dart';
import '../../data/models/product.dart';
import '../../data/product_repository.dart';
import '../cubit/products_cubit.dart';

/// The catalog table: search, add, and the tenant's products. Rendered inside the
/// application shell, which supplies the top bar and navigation.
class ProductsPage extends StatelessWidget {
  const ProductsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final locale = Localizations.localeOf(context).languageCode;
    return BlocProvider(
      create: (_) => ProductsCubit(sl<ProductRepository>())..load(locale),
      child: _ProductsView(locale: locale),
    );
  }
}

class _ProductsView extends StatelessWidget {
  const _ProductsView({required this.locale});

  final String locale;

  static const double _wideTable = 640;

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<ProductsCubit, ProductsState>(
      builder: (context, state) {
        return Column(
          children: [
            _Toolbar(state: state, onAdd: () => _openCreateForm(context)),
            Expanded(
              child: switch (state.status) {
                ProductsStatus.loading || ProductsStatus.initial =>
                  const Center(child: CircularProgressIndicator()),
                ProductsStatus.error => _ErrorView(
                  onRetry: () => context.read<ProductsCubit>().load(locale),
                ),
                ProductsStatus.loaded => _Results(
                  state: state,
                  locale: locale,
                  onAdd: () => _openCreateForm(context),
                ),
              },
            ),
          ],
        );
      },
    );
  }

  Future<void> _openCreateForm(BuildContext context) async {
    final cubit = context.read<ProductsCubit>();
    final created = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      builder: (_) => _CreateProductSheet(locale: locale),
    );
    if (created == true) {
      await cubit.load(locale);
    }
  }
}

class _Toolbar extends StatelessWidget {
  const _Toolbar({required this.state, required this.onAdd});

  final ProductsState state;
  final VoidCallback onAdd;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final narrow = MediaQuery.sizeOf(context).width < 720;

    final search = TextField(
      onChanged: context.read<ProductsCubit>().search,
      decoration: InputDecoration(
        hintText: l10n.searchProductsHint,
        prefixIcon: const Icon(Icons.search, size: 20),
        isDense: true,
        contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 14),
      ),
    );

    final addButton = FilledButton.icon(
      onPressed: onAdd,
      icon: const Icon(Icons.add),
      label: Text(l10n.addProduct),
    );

    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 18, 20, 14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              if (state.status == ProductsStatus.loaded)
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 5,
                  ),
                  decoration: BoxDecoration(
                    color: scheme.secondaryContainer,
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    l10n.itemCount(state.visibleProducts.length),
                    style: Theme.of(context).textTheme.labelMedium?.copyWith(
                      color: scheme.onSecondaryContainer,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
              const Spacer(),
              if (!narrow) ...[
                SizedBox(width: 280, child: search),
                const SizedBox(width: 12),
              ],
              addButton,
            ],
          ),
          if (narrow) ...[const SizedBox(height: 12), search],
        ],
      ),
    );
  }
}

class _Results extends StatelessWidget {
  const _Results({
    required this.state,
    required this.locale,
    required this.onAdd,
  });

  final ProductsState state;
  final String locale;
  final VoidCallback onAdd;

  @override
  Widget build(BuildContext context) {
    if (state.products.isEmpty) {
      return _EmptyView(onAdd: onAdd);
    }

    final rows = state.visibleProducts;
    if (rows.isEmpty) {
      return const _NoMatchesView();
    }

    final wide =
        MediaQuery.sizeOf(context).width >= _ProductsView._wideTable;

    return Column(
      children: [
        if (wide) const _TableHeader(),
        Expanded(
          child: ListView.separated(
            padding: const EdgeInsets.fromLTRB(20, 0, 20, 24),
            itemCount: rows.length,
            separatorBuilder: (context, _) => Divider(
              height: 1,
              color: Theme.of(context).colorScheme.outlineVariant,
            ),
            itemBuilder: (context, index) =>
                _ProductRow(product: rows[index], locale: locale, wide: wide),
          ),
        ),
      ],
    );
  }
}

class _TableHeader extends StatelessWidget {
  const _TableHeader();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final style = Theme.of(context).textTheme.labelMedium?.copyWith(
      color: scheme.onSurfaceVariant,
      fontWeight: FontWeight.w700,
    );

    return Container(
      padding: const EdgeInsets.fromLTRB(20, 0, 20, 0),
      color: scheme.surfaceContainerLow,
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 11),
        child: Row(
          children: [
            const SizedBox(width: 52),
            Expanded(flex: 4, child: Text(l10n.columnProduct, style: style)),
            Expanded(flex: 2, child: Text(l10n.columnSku, style: style)),
            Expanded(
              flex: 2,
              child: Text(
                l10n.columnPrice,
                style: style,
                textAlign: TextAlign.right,
              ),
            ),
            const SizedBox(width: 16),
            // Flexible rather than a fixed width: the status word length varies by
            // language (for example "Cyabitswe" in Kinyarwanda).
            Expanded(
              flex: 2,
              child: Text(
                l10n.columnStatus,
                style: style,
                textAlign: TextAlign.right,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ProductRow extends StatelessWidget {
  const _ProductRow({
    required this.product,
    required this.locale,
    required this.wide,
  });

  final Product product;
  final String locale;
  final bool wide;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final price =
        '${NumberFormat.decimalPattern(locale).format(product.priceAmount)} ${product.currencyCode}';

    final icon = Container(
      width: 38,
      height: 38,
      decoration: BoxDecoration(
        color: scheme.primaryContainer,
        borderRadius: BorderRadius.circular(10),
      ),
      child: Icon(
        Icons.inventory_2_outlined,
        size: 19,
        color: scheme.onPrimaryContainer,
      ),
    );

    if (!wide) {
      return Padding(
        padding: const EdgeInsets.symmetric(vertical: 12),
        child: Row(
          children: [
            icon,
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    product.name,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
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
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Text(
                  price,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 4),
                _StatusChip(isActive: product.isActive),
              ],
            ),
          ],
        ),
      );
    }

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 11),
      child: Row(
        children: [
          icon,
          const SizedBox(width: 14),
          Expanded(
            flex: 4,
            child: Text(
              product.name,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(
                context,
              ).textTheme.bodyLarge?.copyWith(fontWeight: FontWeight.w600),
            ),
          ),
          Expanded(
            flex: 2,
            child: Text(
              product.sku,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: scheme.onSurfaceVariant,
              ),
            ),
          ),
          Expanded(
            flex: 2,
            child: Text(
              price,
              textAlign: TextAlign.right,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(
                context,
              ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w700),
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            flex: 2,
            child: Align(
              alignment: Alignment.centerRight,
              child: _StatusChip(isActive: product.isActive),
            ),
          ),
        ],
      ),
    );
  }
}

/// State is carried by an icon and a label, never by colour alone.
class _StatusChip extends StatelessWidget {
  const _StatusChip({required this.isActive});

  final bool isActive;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    final background = isActive
        ? scheme.primaryContainer
        : scheme.surfaceContainerHighest;
    final foreground = isActive
        ? scheme.onPrimaryContainer
        : scheme.onSurfaceVariant;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 9, vertical: 4),
      decoration: BoxDecoration(
        color: background,
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            isActive ? Icons.check_circle : Icons.inventory_2_outlined,
            size: 13,
            color: foreground,
          ),
          const SizedBox(width: 5),
          Flexible(
            child: Text(
              isActive ? l10n.statusActive : l10n.statusArchived,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                color: foreground,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _EmptyView extends StatelessWidget {
  const _EmptyView({required this.onAdd});

  final VoidCallback onAdd;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 72,
              height: 72,
              decoration: BoxDecoration(
                color: scheme.surfaceContainerHighest,
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.inventory_2_outlined,
                size: 32,
                color: scheme.onSurfaceVariant,
              ),
            ),
            const SizedBox(height: 18),
            Text(
              l10n.noProductsTitle,
              style: Theme.of(
                context,
              ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 6),
            Text(
              l10n.noProductsHint,
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: scheme.onSurfaceVariant,
              ),
            ),
            const SizedBox(height: 20),
            FilledButton.icon(
              onPressed: onAdd,
              icon: const Icon(Icons.add),
              label: Text(l10n.addProduct),
            ),
          ],
        ),
      ),
    );
  }
}

class _NoMatchesView extends StatelessWidget {
  const _NoMatchesView();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.search_off, size: 40, color: scheme.onSurfaceVariant),
            const SizedBox(height: 12),
            Text(
              l10n.noSearchResults,
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: scheme.onSurfaceVariant,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ErrorView extends StatelessWidget {
  const _ErrorView({required this.onRetry});

  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(l10n.productsLoadError),
          const SizedBox(height: 12),
          OutlinedButton(onPressed: onRetry, child: Text(l10n.retryButton)),
        ],
      ),
    );
  }
}

class _CreateProductSheet extends StatefulWidget {
  const _CreateProductSheet({required this.locale});

  final String locale;

  @override
  State<_CreateProductSheet> createState() => _CreateProductSheetState();
}

class _CreateProductSheetState extends State<_CreateProductSheet> {
  final _formKey = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _sku = TextEditingController();
  final _price = TextEditingController();
  final _currency = TextEditingController(text: 'RWF');
  bool _submitting = false;
  String? _error;

  @override
  void dispose() {
    _name.dispose();
    _sku.dispose();
    _price.dispose();
    _currency.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context);
    if (!_formKey.currentState!.validate()) {
      return;
    }
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      await sl<ProductRepository>().create(
        name: _name.text.trim(),
        sku: _sku.text.trim(),
        priceAmount: double.parse(_price.text.trim()),
        currencyCode: _currency.text.trim().toUpperCase(),
        locale: widget.locale,
      );
      if (mounted) {
        Navigator.of(context).pop(true);
      }
    } on ApiException catch (error) {
      setState(() {
        _submitting = false;
        _error = error.code == ApiErrorCodes.network
            ? l10n.errorNetwork
            : error.message;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final viewInsets = MediaQuery.of(context).viewInsets.bottom;
    return Padding(
      padding: EdgeInsets.fromLTRB(24, 24, 24, 24 + viewInsets),
      child: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 520),
          child: Form(
            key: _formKey,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  l10n.addProduct,
                  style: Theme.of(
                    context,
                  ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
                ),
                const SizedBox(height: 16),
                if (_error != null) ...[
                  InfoBanner(message: _error!),
                  const SizedBox(height: 16),
                ],
                TextFormField(
                  controller: _name,
                  decoration: InputDecoration(
                    labelText: l10n.productNameLabel,
                    prefixIcon: const Icon(Icons.inventory_2_outlined),
                  ),
                  validator: (v) =>
                      (v == null || v.trim().isEmpty) ? l10n.fieldRequired : null,
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _sku,
                  decoration: InputDecoration(
                    labelText: l10n.productSkuLabel,
                    prefixIcon: const Icon(Icons.tag_outlined),
                  ),
                  validator: (v) =>
                      (v == null || v.trim().isEmpty) ? l10n.fieldRequired : null,
                ),
                const SizedBox(height: 12),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      flex: 2,
                      child: TextFormField(
                        controller: _price,
                        keyboardType: const TextInputType.numberWithOptions(
                          decimal: true,
                        ),
                        decoration: InputDecoration(
                          labelText: l10n.productPriceLabel,
                          prefixIcon: const Icon(Icons.payments_outlined),
                        ),
                        validator: (v) {
                          final parsed = double.tryParse((v ?? '').trim());
                          return (parsed == null || parsed < 0)
                              ? l10n.fieldInvalidFormat
                              : null;
                        },
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: TextFormField(
                        controller: _currency,
                        decoration: InputDecoration(
                          labelText: l10n.currencyCodeLabel,
                        ),
                        validator: (v) => (v != null && v.trim().length == 3)
                            ? null
                            : l10n.fieldInvalidFormat,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                FilledButton(
                  onPressed: _submitting ? null : _submit,
                  child: _submitting
                      ? Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            const SizedBox(
                              height: 18,
                              width: 18,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            ),
                            const SizedBox(width: 12),
                            Text(l10n.creatingButton),
                          ],
                        )
                      : Text(l10n.createProductButton),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
