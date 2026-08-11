# ADR-007: Next.js for the web dashboard

## Status

Accepted.

## Context

CLAUDE.md names Flutter as the frontend for every target, and a Flutter client
exists under `frontend/` covering sign-up, sign-in, the dashboard and the catalog.

Two things pushed the browser experience away from Flutter:

- Flutter web renders to canvas. Text is not real DOM, which costs SEO, browser
  text selection and find-in-page, assistive-technology fidelity, and the ability
  to drive the UI with ordinary web tooling.
- The dashboard is the surface where a conventional web stack pays off most:
  server rendering, links that behave like links, and a large ecosystem of
  data-table and charting components.

## Decision

The **web dashboard** is built with Next.js (App Router) in `web/`, using
TypeScript, Tailwind CSS, Mantine, Framer Motion and Lucide icons.

Flutter remains the client for **mobile and desktop** targets, and `frontend/`
stays in the repository. This is an addition, not a removal.

Both clients speak the same HTTP contract, so the backend is unchanged and stays
the single source of business behaviour.

## Consequences

- Two client codebases now exist. Anything user-visible — copy, validation
  messages, permissions — must be kept consistent across them, and the API
  contract is what keeps them honest.
- The web client renders on the server, so the access token lives in an httpOnly
  cookie and is never exposed to browser JavaScript. This is stronger than the
  Flutter web build, which necessarily kept it in browser storage.
- Localisation is duplicated: ARB files for Flutter, dictionary modules for
  Next.js. Both cover en, rw and fr; a shared translation source is a candidate
  for later.
- If the web client eventually covers every use case, retiring the Flutter web
  target (not the mobile targets) becomes a separate, reversible decision.

## Alternatives considered

- **Keep Flutter web only.** Rejected: the canvas renderer's SEO and
  accessibility limits are hard to work around for a public-facing dashboard.
- **Replace Flutter entirely.** Rejected for now: the mobile and offline story
  matters for field use, and discarding working clients has no upside today.
