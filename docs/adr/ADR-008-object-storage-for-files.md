# ADR-008: S3-compatible object storage, hosted on Cloudflare R2

## Status

Accepted. No implementation yet — this records the decision so the first feature
that needs files does not have to make it under pressure.

## Context

`docs/03-architecture/10-storage-architecture.md` was empty, so nothing said where
uploaded bytes belong. Product images are the first feature to need an answer, and
receipts, delivery photos and exports will follow.

Two constraints shape the choice:

- **Multi-tenancy.** Files are tenant data. One business must not be able to reach
  another's, including by guessing a URL.
- **Bandwidth.** The first customers are Rwandan retailers on mobile data. Images
  are fetched far more often than they are uploaded, so egress cost and payload
  size matter more than storage cost.

## Decision

Files go in **S3-compatible object storage**, hosted on **Cloudflare R2**.

- The application depends on the **S3 API**, not on R2, behind an internal
  abstraction. Moving to AWS S3 or self-hosted MinIO is a configuration change.
- Object keys are **tenant-prefixed** (`{tenantId}/{module}/{entityId}/{fileId}`),
  and buckets are private. Reads go through **signed, expiring URLs**; there are no
  public object URLs.
- Uploads use **presigned PUT** direct to storage, so image bytes never pass
  through the API. The API authorises the upload and records the resulting key.
- Every stored file is validated on content type and size before its key is
  accepted, and each tenant has a quota tied to its plan.

## Consequences

- R2 charges no egress, which is the dominant cost when phones repeatedly fetch
  product images. This is the main reason it was chosen over S3.
- Presigned uploads mean the API never sees the bytes, but it also means an
  authorised client could upload something other than what it declared. Size and
  content type are therefore re-checked before the key is attached to a record.
- Local development needs an S3-compatible endpoint. MinIO in the existing Docker
  compose covers this without a cloud account.
- Derivatives (thumbnail, card, full) are generated rather than serving originals.
  Where that happens — on upload or on demand at the edge — is deferred to the
  implementing ADR.
