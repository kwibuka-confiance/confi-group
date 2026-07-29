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

/// Lists the tenant's products and lets the user add one.
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
    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.productsTitle),
        actions: const [ThemeMenuButton(), SizedBox(width: 4)],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _openCreateForm(context),
        icon: const Icon(Icons.add),
        label: Text(l10n.addProduct),
      ),
      body: BlocBuilder<ProductsCubit, ProductsState>(
        builder: (context, state) {
          return switch (state.status) {
            ProductsStatus.loading || ProductsStatus.initial =>
              const Center(child: CircularProgressIndicator()),
            ProductsStatus.error => _ErrorView(
              onRetry: () => context.read<ProductsCubit>().load(locale),
            ),
            ProductsStatus.loaded =>
              state.products.isEmpty
                  ? const _EmptyView()
                  : _ProductList(products: state.products, locale: locale),
          };
        },
      ),
    );
  }

  Future<void> _openCreateForm(BuildContext context) async {
    final created = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      builder: (_) => _CreateProductSheet(locale: locale),
    );
    if (created == true && context.mounted) {
      await context.read<ProductsCubit>().load(locale);
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
    return Center(
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 720),
        child: ListView.separated(
          padding: const EdgeInsets.all(16),
          itemCount: products.length,
          separatorBuilder: (_, __) => const SizedBox(height: 8),
          itemBuilder: (context, index) {
            final product = products[index];
            return Card(
              child: ListTile(
                leading: CircleAvatar(
                  backgroundColor: scheme.primaryContainer,
                  child: Icon(
                    Icons.inventory_2_outlined,
                    color: scheme.onPrimaryContainer,
                  ),
                ),
                title: Text(product.name),
                subtitle: Text(product.sku),
                trailing: Text(
                  _formatPrice(product, locale),
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
            );
          },
        ),
      ),
    );
  }

  static String _formatPrice(Product product, String locale) {
    final amount = NumberFormat.decimalPattern(locale).format(product.priceAmount);
    return '$amount ${product.currencyCode}';
  }
}

class _EmptyView extends StatelessWidget {
  const _EmptyView();

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
            Icon(Icons.inventory_2_outlined, size: 56, color: scheme.onSurfaceVariant),
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
                    keyboardType: const TextInputType.numberWithOptions(decimal: true),
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
                    decoration: InputDecoration(labelText: l10n.currencyCodeLabel),
                    validator: (v) =>
                        (v != null && v.trim().length == 3)
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
    );
  }
}
