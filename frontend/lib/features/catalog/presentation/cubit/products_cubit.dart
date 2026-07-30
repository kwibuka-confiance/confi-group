import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_exception.dart';
import '../../data/models/product.dart';
import '../../data/product_repository.dart';

enum ProductsStatus { initial, loading, loaded, error }

class ProductsState extends Equatable {
  const ProductsState({
    this.status = ProductsStatus.initial,
    this.products = const [],
    this.query = '',
  });

  final ProductsStatus status;

  /// Everything loaded for the tenant, before the search box is applied.
  final List<Product> products;

  final String query;

  /// The rows to show: name or SKU containing the query, case-insensitively.
  List<Product> get visibleProducts {
    if (query.trim().isEmpty) {
      return products;
    }
    final needle = query.trim().toLowerCase();
    return products
        .where(
          (product) =>
              product.name.toLowerCase().contains(needle) ||
              product.sku.toLowerCase().contains(needle),
        )
        .toList();
  }

  bool get isFiltering => query.trim().isNotEmpty;

  ProductsState copyWith({
    ProductsStatus? status,
    List<Product>? products,
    String? query,
  }) {
    return ProductsState(
      status: status ?? this.status,
      products: products ?? this.products,
      query: query ?? this.query,
    );
  }

  @override
  List<Object?> get props => [status, products, query];
}

/// Loads the tenant's products and holds the search term. Filtering happens on
/// the loaded list, so typing does not re-query the API.
class ProductsCubit extends Cubit<ProductsState> {
  ProductsCubit(this._repository) : super(const ProductsState());

  final ProductRepository _repository;

  Future<void> load(String locale) async {
    emit(state.copyWith(status: ProductsStatus.loading));
    try {
      final products = await _repository.list(locale: locale);
      emit(state.copyWith(status: ProductsStatus.loaded, products: products));
    } on ApiException {
      emit(state.copyWith(status: ProductsStatus.error));
    }
  }

  void search(String query) => emit(state.copyWith(query: query));
}
