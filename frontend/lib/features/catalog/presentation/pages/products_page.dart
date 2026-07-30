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

/// Lists the tenant's products and lets the user add one. Rendered inside the
/// application shell, which supplies the app bar and navigation.
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

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return BlocBuilder<ProductsCubit, ProductsState>(
      builder: (context, state) {
        return Column(
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(24, 20, 24, 12),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      state.status == ProductsStatus.loaded
                          ? '${state.products.length} ${l10n.productsTitle.toLowerCase()}'
                          : l10n.productsTitle,
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  FilledButton.icon(
                    onPressed: () => _openCreateForm(context),
                    icon: const Icon(Icons.add),
                    label: Text(l10n.addProduct),
                  ),
                ],
              ),
            ),
            Expanded(
              child: switch (state.status) {
                ProductsStatus.loading || ProductsStatus.initial =>
                  const Center(child: CircularProgressIndicator()),
                ProductsStatus.error => _ErrorView(
                  onRetry: () => context.read<ProductsCubit>().load(locale),
                ),
                ProductsStatus.loaded =>
                  state.products.isEmpty
                      ? _EmptyView(onAdd: () => _openCreateForm(context))
                      : _ProductList(products: state.products, locale: locale),
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

class _ProductList extends StatelessWidget {
  const _ProductList({required this.products, required this.locale});

  final List<Product> products;
  final String locale;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(24, 0, 24, 24),
      itemCount: products.length,
      separatorBuilder: (_, __) => const SizedBox(height: 8),
      itemBuilder: (context, index) {
        final product = products[index];
        final amount = NumberFormat.decimalPattern(
          locale,
        ).format(product.priceAmount);

        return Container(
          decoration: BoxDecoration(
            color: scheme.surfaceContainerLow,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: scheme.outlineVariant),
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            child: Row(
              children: [
                Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: scheme.primaryContainer,
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(
                    Icons.inventory_2_outlined,
                    size: 20,
                    color: scheme.onPrimaryContainer,
                  ),
                ),
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
                Text(
                  '$amount ${product.currencyCode}',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
        );
      },
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
            Icon(
              Icons.inventory_2_outlined,
              size: 56,
              color: scheme.onSurfaceVariant,
            ),
            const SizedBox(height: 16),
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
