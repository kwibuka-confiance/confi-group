# Contributing to ConfiOS

## Workflow

1. Create or update the relevant requirement.
2. Create a branch from `develop`.
3. Implement the smallest complete feature.
4. Add or update tests.
5. Update documentation.
6. Open a pull request.
7. Obtain review before merging.

## Branch Naming

- `feature/<short-description>`
- `fix/<short-description>`
- `docs/<short-description>`
- `refactor/<short-description>`

## Pull Request Expectations

Every pull request should explain:

- Problem being solved
- Requirement IDs
- Architecture impact
- Security impact
- Localization impact
- Testing performed
- Screenshots for UI work

## Coding Expectations

- Prefer clarity over cleverness.
- Keep functions focused.
- Avoid shared mutable state.
- Avoid premature abstractions.
- Use domain language consistently.
- Never bypass tenant or permission checks.
