/**
 * Role model derived from the project diagram.
 *
 * Two separate hierarchies exist on purpose — they are NOT the same tree:
 *
 * 1. Donor-side role (gates UI inside the `user` domain only):
 *      Donor -> Worker
 *    A worker is just a user with an extra permission flag. They see the
 *    same /user routes, plus extra worker-only views inside the
 *    "donated items" flow (ask for donated items, confirm receipt, mark
 *    item status). This is intentionally NOT a separate domain.
 *
 * 2. Platform admin hierarchy (lives entirely in the `admin` domain):
 *      Ultra Super Admin -> Super Admin of Org -> Sub Admin -> Worker
 *    This tree manages ORGS from outside — creating orgs, editing them,
 *    promoting users to admin of a specific org, payment keys. It is a
 *    different domain and a different permission level than an org
 *    managing its own campaigns (that's the `org` domain).
 *
 * NOTE: the diagram's admin tree also ends in "Worker" at the bottom.
 * We're treating that as the SAME worker role as (1) — i.e. the admin
 * hierarchy is how a worker gets assigned, but a worker's actual job
 * (confirming donated items) happens in the `user` domain's UI. If it
 * turns out the diagram means two distinct kinds of "worker", this enum
 * needs to split into UserRole.Worker and AdminRole.Worker separately.
 */

export enum UserRole {
  Donor = 'donor',
  Worker = 'worker',
}

export enum AdminRole {
  SubAdmin = 'sub_admin',
  SuperAdminOfOrg = 'super_admin_of_org',
  UltraSuperAdmin = 'ultra_super_admin',
}

/** Numeric rank for "is this admin role at least as senior as X" checks. */
export const ADMIN_ROLE_RANK: Record<AdminRole, number> = {
  [AdminRole.SubAdmin]: 1,
  [AdminRole.SuperAdminOfOrg]: 2,
  [AdminRole.UltraSuperAdmin]: 3,
};

export function isAtLeast(role: AdminRole, minimum: AdminRole): boolean {
  return ADMIN_ROLE_RANK[role] >= ADMIN_ROLE_RANK[minimum];
}
