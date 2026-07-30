import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_exception.dart';
import '../../../catalog/data/models/product.dart';
import '../../../catalog/data/product_repository.dart';

enum DashboardStatus { loading, loaded, error }

class DashboardState extends Equatable {
  const DashboardState({
    this.status = DashboardStatus.loading,
    this.totalProducts = 0,
    this.activeProducts = 0,
    this.valueByCurrency = const {},
    this.recentProducts = const [],
  });

  final DashboardStatus status;
  final int totalProducts;
  final int activeProducts;

  /// Catalog value per currency. Amounts in different currencies are never added
  /// together, so they stay separated here and the UI reports accordingly.
  final Map<String, double> valueByCurrency;

  final List<Product> recentProducts;

  @override
  List<Object?> get props => [
    status,
    totalProducts,
    activeProducts,
    valueByCurrency,
    recentProducts,
  ];
}

/// Derives the dashboard's figures from the catalog. Everything shown is computed
/// from data the API actually returned — nothing is estimated.
class DashboardCubit extends Cubit<DashboardState> {
  DashboardCubit(this._products) : super(const DashboardState());

  static const int _recentCount = 5;

  final ProductRepository _products;

  Future<void> load(String locale) async {
    emit(const DashboardState());
    try {
      final products = await _products.list(locale: locale);

      final valueByCurrency = <String, double>{};
      for (final product in products) {
        valueByCurrency.update(
          product.currencyCode,
          (total) => total + product.priceAmount,
          ifAbsent: () => product.priceAmount,
        );
      }

      // Ids are UUIDv7, so descending id order is newest first.
      final recent = [...products]..sort((a, b) => b.id.compareTo(a.id));

      emit(
        DashboardState(
          status: DashboardStatus.loaded,
          totalProducts: products.length,
          activeProducts: products.where((product) => product.isActive).length,
          valueByCurrency: valueByCurrency,
          recentProducts: recent.take(_recentCount).toList(),
        ),
      );
    } on ApiException {
      emit(const DashboardState(status: DashboardStatus.error));
    }
  }
}
