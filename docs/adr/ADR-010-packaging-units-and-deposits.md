# ADR-010: Packaging as units of measure, and returnable deposits

## Status

Accepted. Supersedes the packaging paragraph of ADR-009.

## Context

A depot sells the same thing several ways. Mutsig Large is sold by the bottle and
by the case of 12; Mutsig Mid comes 20 to a case and Mutsig Small 24. The pack
size is a property of the product, and it differs product by product.

Two modelling mistakes are easy to make here:

1. **Treating a pack as a separate product.** "Mutsig case" and "Mutsig bottle" as
   two products means two stock pools. Selling twelve bottles leaves the case
   count untouched, so every opened case needs a manual adjustment, and the first
   time somebody forgets, the stock figures stop being true.
2. **Treating the container as free.** In beverage distribution the crate and the
   bottle are returnable and carry a deposit. Money taken for a deposit is money
   that may have to be handed back, and the empties themselves are stock sitting
   in the yard.

Both are cheap to design for now and expensive to retrofit once sales history
exists.

## Decision

### Packaging is a unit of measure

A product has one **base unit** that stock is held in, plus any number of
**packagings** that convert to it.

- `Mutsig Large 650ml` — base unit `BOTTLE`; packaging `CASE` = 12 bottles.
- `Mutsig Mid 500ml` — base unit `BOTTLE`; packaging `CASE` = 20 bottles.
- `Mutsig Small 330ml` — base unit `BOTTLE`; packaging `CASE` = 24 bottles.

Stock is always recorded in the base unit. Buying 100 cases of Mutsig Large adds
1,200 bottles; selling one case removes 12; selling three loose bottles removes 3.
There is no break-bulk step, and "how many cases do I have" is a division rather
than a separate number that can drift.

Each packaging carries **its own selling and cost price** and **its own barcode**,
because a case is priced below twelve singles and scans to a different code. This
is why the domain model lists barcodes and prices in the plural.

### Containers are catalog items with a deposit

A returnable crate or bottle is itself a catalog item with a deposit value. A
packaging may reference the container it ships in.

- Selling a case issues the container and charges its deposit.
- Returning empties refunds the deposit and takes the containers back into stock.
- Empty containers are ordinary stock: countable, and visible in inventory.

A deposit is a **liability, not revenue**. It is money held that may be repaid, so
reporting must never fold deposits into sales figures.

### This is not a beverage feature

The same shape covers pharmacy (tablet, blister of 10, box of 100), hardware
(screw, box of 50, pallet) and textiles (metre, roll of 50m). Deposits likewise
cover gas cylinders and pallets. "Case" and "crate" are data a tenant enters, not
concepts in the platform, which keeps CLAUDE.md's industry-agnostic rule intact.

## Consequences

- Every quantity that crosses a module boundary must carry its unit. A sales line
  is quantity plus unit, not a bare number, and conversion to the base unit
  happens once, at the edge of the domain.
- Inventory holds one figure per product, in the base unit, and reports it in
  whichever unit the user asked for.
- Sales gains deposit lines distinct from product lines, and a path for returning
  empties that is not a refund of a sale.
- Reporting must separate revenue from deposit movements, or margin will be wrong.
- Changing a packaging's conversion after stock exists rewrites the meaning of
  historical quantities, so conversions are fixed once a product has movements.
