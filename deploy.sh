#!/usr/bin/env bash
# Deploy Mod/ into the local RimWorld Mods folder as QuadrumYears.
# Full wipe-and-replace of the install folder; use after changing non-assembly
# loadable content (About/, LoadFolders.xml, defs, etc.). C# builds only refresh
# Assemblies under Mod/<version>/ and the live install's matching Assemblies path.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="${REPO_ROOT}/paths.env"
MOD_SOURCE="${REPO_ROOT}/Mod"
MOD_NAME="QuadrumYears"

if [[ ! -f "${ENV_FILE}" ]]; then
	echo "Missing ${ENV_FILE}" >&2
	echo "Copy paths.env.example to paths.env and set RIMWORLD_MODS_DIR." >&2
	exit 1
fi

# shellcheck disable=SC1090
source "${ENV_FILE}"

if [[ -z "${RIMWORLD_MODS_DIR:-}" ]]; then
	echo "RIMWORLD_MODS_DIR is not set in ${ENV_FILE}" >&2
	exit 1
fi

if [[ ! -d "${RIMWORLD_MODS_DIR}" ]]; then
	echo "RimWorld Mods directory does not exist: ${RIMWORLD_MODS_DIR}" >&2
	exit 1
fi

if [[ ! -d "${MOD_SOURCE}" ]]; then
	echo "Mod source directory missing: ${MOD_SOURCE}" >&2
	exit 1
fi

DEST="${RIMWORLD_MODS_DIR%/}/${MOD_NAME}"

# Refuse to wipe anything that isn't clearly this mod's install folder.
if [[ "$(basename "${DEST}")" != "${MOD_NAME}" ]]; then
	echo "Refusing to deploy to unexpected path: ${DEST}" >&2
	exit 1
fi

echo "Deploying ${MOD_SOURCE} -> ${DEST}"

if [[ -e "${DEST}" ]]; then
	echo "Clearing existing install at ${DEST}"
	rm -rf "${DEST}"
fi

mkdir -p "${DEST}"
cp -a "${MOD_SOURCE}/." "${DEST}/"

echo "Done."
