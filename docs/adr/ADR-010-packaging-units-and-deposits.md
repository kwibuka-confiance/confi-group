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

- `Mutsig Large 650ml` — base unit `BOTTLE`; packaging `CRATE` = 12 bottles.
- `Mutsig Mid 500ml` — base unit `BOTTLE`; packaging `CRATE` = 20 bottles.
- `Mutsig Small 330ml` — base unit `BOTTLE`; packaging `CRATE` = 24 bottles.

Selling happens in crates and in quarters of a crate, so a sale of three quarters
of Mutsig Small is 18 bottles. The bottle stays the counting unit because it is
the indivisible physical thing; every screen still speaks in crates.

A quarter only exists if it resolves to whole bottles. Twelve, twenty and
twenty-four all divide by four, but a crate of ten would make a quarter two and a
half bottles. A pack size that cannot be quartered must not offer quarters, and
that is checked when the product is set up rather than at the counter.

Stock is always recorded in the base unit. Buying 100 cases of Mutsig Large adds
1,200 bottles; selling one case removes 12; selling three loose bottles removes 3.
There is no break-bulk step, and "how many cases do I have" is a division rather
than a separate number that can drift.

Each packaging carries **its own selling and cost price** and **its own barcode**,
because a case is priced below twelve singles and scans to a different code. This
is why the domain model lists barcodes and prices in the plural.

### The crate is a unit, not a container that moves

At KwaConfi no crate leaves the depot. Customers arrive with their own crate,
hand over empty bottles and leave with filled ones in the same crate. A part-crate
(a quarter, half or three quarters) goes out as loose bottles into whatever the
customer brought.

So `CRATE` is a **unit of measure only** — a way of saying "24 bottles" for
counting and pricing. It is not modelled as a container, carries no deposit, and
is never tracked against a customer.

### The bottle is the returnable, tracked as a balance

The exchange is bottle for bottle, so what matters is **how many bottles a
customer is holding**, not a payment on every transaction.

- Bring 24 empties, take 24 full → balance unchanged, the customer pays for the
  drink only.
- Bring 18, take 24 → the customer now holds 6 more of your bottles.
- Bring 30, take 24 → you hold 6 of theirs.

Each customer therefore has a **running bottle balance**. A deposit is the way an
imbalance is settled, not something charged on every sale. Empty bottles held at
the depot are ordinary stock and are counted like anything else.

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
- Sales records the empties handed in alongside what is taken out, and adjusts the
  customer's bottle balance by the difference. Returning empties is its own
  transaction, not a refund of a sale.
- Customers gain a bottle balance, which is where the exchange is actually
  settled.
- Reporting must separate revenue from deposit movements, or margin will be wrong.
- Changing a packaging's conversion after stock exists rewrites the meaning of
  historical quantities, so conversions are fixed once a product has movements.
