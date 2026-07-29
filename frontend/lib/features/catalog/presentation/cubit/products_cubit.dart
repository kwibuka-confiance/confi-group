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
  });

  final ProductsStatus status;
  final List<Product> products;

  @override
  List<Object?> get props => [status, products];
}

/// Loads the tenant's products. Creating a product is handled in the form and
/// followed by [load] to refresh.
class ProductsCubit extends Cubit<ProductsState> {
  ProductsCubit(this._repository) : super(const ProductsState());

  final ProductRepository _repository;

  Future<void> load(String locale) async {
    emit(const ProductsState(status: ProductsStatus.loading));
    try {
      final products = await _repository.list(locale: locale);
      emit(ProductsState(status: ProductsStatus.loaded, products: products));
    } on ApiException {
      emit(const ProductsState(status: ProductsStatus.error));
    }
  }
}
