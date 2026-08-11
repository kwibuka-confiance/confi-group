# ADR-009: Products are flat; no variants

## Status

Accepted.

## Context

A catalog can model a sellable thing in two ways: one product per sellable thing,
each with its own SKU; or a parent product with variant children that carry their
own SKU, price and barcode.

Variants are the right model for apparel, where "T-shirt" in five sizes and four
colours is twenty sellable things that share a name and an image. They are
unnecessary for a depot, where a crate of water and a bottle of water are simply
two products.

## Decision

**A product is flat.** Every sellable thing is its own product with its own SKU,
price and barcodes. There is no parent/variant relationship.

Pack sizes are expressed through the unit and pack fields on the product itself
(`unitCode`, `unitsPerPack`), not through variants — a crate of 24 is a product
whose unit is CRATE and whose pack size is 24.

## Consequences

- Inventory, pricing and sales all address a single product identifier. Every
  module downstream stays simpler, which is the point.
- Businesses with genuine variant catalogs must create one product per
  combination. For the first customers this is acceptable; for apparel retail it
  would not be.
- Introducing variants later is a real migration, not an additive change: it
  touches the catalog schema and every module that references a product. Should
  that become necessary, it warrants its own ADR rather than being retrofitted
  quietly.
