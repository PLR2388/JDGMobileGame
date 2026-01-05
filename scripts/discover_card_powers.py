#!/usr/bin/env python3
"""
Card Power Discovery Agent
Parses all card ScriptableObjects and extracts abilities to generate a comprehensive catalog.
"""

import os
import re
import json
from pathlib import Path
from collections import defaultdict
from dataclasses import dataclass, field
from typing import List, Dict, Set, Optional

# Enum definitions based on JDG.Domain enums
CARD_FAMILIES = [
    "None", "Comics", "Developer", "Fistiland", "HardCorner", "Human",
    "Incarnation", "Japan", "Monster", "Police", "Rpg", "Spatial", "Wizard", "Any"
]

INVOCATION_ABILITIES = [
    "CanOnlyAttackItself", "AddSpatialFromDeck", "SacrificeArchibaldVonGrenier",
    "CantBeAttackIfComics", "CantLiveWithoutBenzaieOrBenzaieJeune", "GiveAtkDefToComics",
    "SendAllCardToHands", "SacrificeBenzaieJeune", "GetNounoursFromDeck",
    "SacrificeJoueurDuGrenier", "GetPetitePortionDeRizFromDeck", "InvokeTentacules",
    "GetLyceeMagiqueGeorgesPompidouFromDeck", "SacrificeSebDuGrenierOnHardCornerForAtkDef",
    "Win1Atk1DefDeveloper", "Sacrifice3Atk3Def", "ChangeFieldWithFieldFromDeck",
    "Win1ATK1DefJaponWith2ATK2DEFCondition", "InvokeDresseurBidulmon", "GetZozanKebabFromDeck",
    "SacrificeWizard", "GetConvocationAuLyceeFromDeck", "ProtectedBehindStarlightUnicorn",
    "GetCanardSignal", "SacrificeDeveloper3Atk3Def", "SacrificeHardCorner3Atk3Def",
    "CantBeAttackKill", "ComesBackFromDeath", "Sacrifice2Japan", "DestroyFieldATK",
    "KillOpponentInvocation", "CantLiveWithoutJDG", "GetForetElfesSylvains",
    "InvokeSebOrJDG", "CantLiveWithoutComics", "Sacrifice2Incarnation", "DestroyFieldDEF",
    "GetBenzaieJeuneFromDeck", "GetEquipmentCardWithoutAttack", "SacrificeGranolax",
    "SacrificeJDGOnStudioDevForAtkDef", "CantLiveWithoutHuman", "CopyBenzaieJeune",
    "SurviveOneTurn", "GiveDeathWhenDie", "ProtectBehindGreaterDef", "SacrificeSebDuGrenier",
    "Win1Atk1DefFistiland", "SacrificeClicheRaciste", "KillEnemyIfDestroy",
    "SacrificeToInvoke", "GetPatronInfogramesFromDeckYellowTrash",
    "CantLiveWithoutGranolaxOrMechaGranolax", "SkipOpponentAttackEveryTurn",
    "ComesBackFromDeath5Times", "CantLiveWithoutJapon", "Draw1Card", "Draw2Cards",
    "Draw3Cards", "GiveAktDefToRpgMember", "GiveAktDefToFistilandMember", "Default"
]

EQUIPMENT_ABILITIES = [
    "Default", "MultiplyDefBy2ButPreventAttack", "Earn1ATKAndMinus1DEF", "DirectAttack",
    "EarnOneQuarterATKPerHandCards", "PreventNewOpponentToAttack", "Remove1ATKAnd1DEF",
    "SetATKToOne", "CantBeAttackByOtherInvocations", "SetDefToZero", "MultiplyAtkBy3",
    "Earn2ATK", "Earn3ATKAndMinus1DEF", "Earn1ATKAnd1DEF", "MultiplyAtkBy2AndDefByHalf",
    "EarnOneQuarterDEFPerHandCards", "SwitchEquipmentCard", "Loose2ATK",
    "ProtectOneTimeFromDestruction", "CancelInvocationAbility"
]

FIELD_ABILITIES = [
    "Default", "Earn1DEFForSpatialFamily", "Earn1HalfDEFAndMinusHalfATKForDevFamily",
    "ChangePatronInfogramFamilyToDev", "ChangeJMBruitagesFamilyToDev",
    "Earn2DEFAndMinusOneATKForIncarnationFamily", "EarnHalfHPPerWizardInvocationEachTurn",
    "Earn1ATKForJapanFamily", "Earn1HalfATKAndMinusHalfDEFForHCFamily", "DrawOneMoreCard",
    "EarnHalfATKAndDefForRpgFamily", "SkipDrawToGetFistilandInvocation",
    "Earn2ATKAndMinus1DEFForComicsFamily"
]

EFFECT_ABILITIES = [
    "Default", "LimitHandCardTo5", "Lose2Point5StarsByInvocations",
    "ApplyFamilyFieldToInvocations", "DestroyAllCardsUnderManyConditions",
    "GetHPFor1Sacrifice3ATKDEFCondition", "DirectAttackIfUnder5HP", "ChangeFieldCardFromDeck",
    "DestroyOneCardByRemovingOneHandCard", "DestroyFieldFor7HalfCost", "Get7HalfHPFor1Sacrifice",
    "GetCardFromYellowDeck", "ManiabilitePourrieSkipAttackForOpponent", "SwitchAtkDef",
    "LookAndOrderDeckCards", "LooseHPBasedOnNumberInvocation", "DestroyEquipmentCard",
    "LookOpponentHandCardsAndChangeIt", "DoubleAttackPerTurn", "InvokeCardFromYellowTrash",
    "DivideDEFOpponentBy2", "Add3ShieldsForUser", "DestroyOpponentInvocationCard",
    "Loose1HPPerOpponentHandCards", "GetBackAllHPBySacrifice5AtkDef",
    "Control1OpponentInvocationCard"
]

CONDITIONS = [
    "BenzaieJeuneOrBenzaieOnField", "ArchibalVonGrenierOnField", "ZozanKebabOnField",
    "BenzaieJeuneCassetteVhsEquiped", "JoueurDuGrenierCanarangEquiped", "ThreeAtk3Def",
    "JoueurDuGrenierOnFieldCondition", "ForetDesElfesSylvainsOnField", "WizardOnField",
    "LyceeMagiqueGeorgesPompidouOnField", "Developer3Atk3Def2Cards", "HardCorner3Atk3Def2Cards",
    "Japan2Cards", "TenDeathYellowTrash", "ComicsOnField", "Incarnation2Cards",
    "GranolaxAlreadyDead", "HumanOnField", "SebDuGrenierMerdePlastiqueBleuEquiped",
    "SebDuGrenierOnField", "ClicheRacisteMerdeRoseEquiped", "MechaGronolaxOrGranolaxOnField",
    "JapanOnField"
]

CARD_TYPES = {
    0: "Contre",
    1: "Effect",
    2: "Equipment",
    3: "Field",
    4: "Invocation"
}

# Script GUIDs to identify card types (from m_Script field in asset files)
SCRIPT_GUIDS = {
    "b32082e37a1071a43ae910280ac85b2b": "Invocation",  # InvocationCard
    "7860eebd348ff8d4eafd19296976741d": "Equipment",    # EquipmentCard
    "fe3350d66d1ba9740bca6993fc91c5f3": "Field",        # FieldCard
    "742b51389cec5e448bfa8bf469f8dd7c": "Contre",       # ContreCard
    "e7db7b3a7aa13b64fbb830b3cb1adbe2": "Effect",       # EffectCard
}

@dataclass
class CardData:
    name: str
    card_type: str
    description: str
    detailed_description: str
    attack: float = 0
    defense: float = 0
    families: List[str] = field(default_factory=list)
    abilities: List[str] = field(default_factory=list)
    conditions: List[str] = field(default_factory=list)
    is_collector: bool = False
    field_family: str = ""  # For field cards


def parse_hex_to_int_list(hex_str: str) -> List[int]:
    """Parse Unity's little-endian hex-encoded int list."""
    if not hex_str or hex_str.strip() == "":
        return []

    # Remove spaces and convert to bytes
    hex_clean = hex_str.strip()

    # Unity encodes as little-endian 4-byte integers
    result = []
    for i in range(0, len(hex_clean), 8):  # 8 hex chars = 4 bytes = 1 int
        chunk = hex_clean[i:i+8]
        if len(chunk) == 8:
            # Little-endian conversion
            bytes_val = bytes.fromhex(chunk)
            val = int.from_bytes(bytes_val, byteorder='little')
            result.append(val)

    return result


def parse_asset_file(filepath: Path) -> Optional[CardData]:
    """Parse a Unity .asset file and extract card data."""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
    except Exception as e:
        print(f"Error reading {filepath}: {e}")
        return None

    # Extract basic fields using regex
    def get_field(pattern, default=""):
        match = re.search(pattern, content, re.MULTILINE)
        return match.group(1).strip() if match else default

    name = get_field(r'm_Name:\s*(.+?)$', filepath.stem)
    title = get_field(r'title:\s*(.+?)$', name)
    description = get_field(r'description:\s*(.+?)$', "")
    detailed_desc = get_field(r'detailedDescription:\s*"?(.+?)"?\s*$', "")

    # Determine card type by script GUID (more reliable than type field)
    script_guid = get_field(r'm_Script:.*?guid:\s*([a-f0-9]+)', "")
    card_type = SCRIPT_GUIDS.get(script_guid, None)

    # Fallback to type field if GUID not recognized
    if not card_type:
        type_val = int(get_field(r'^  type:\s*(\d+)', "0"))
        card_type = CARD_TYPES.get(type_val, "Unknown")

    # Collector status
    collector = get_field(r'collector:\s*(\d+)', "0") == "1"

    card = CardData(
        name=title if title else name,
        card_type=card_type,
        description=description,
        detailed_description=detailed_desc,
        is_collector=collector
    )

    if card_type == "Invocation":
        # Stats
        card.attack = float(get_field(r'Attack:\s*([\d.]+)', "0"))
        card.defense = float(get_field(r'Defense:\s*([\d.]+)', "0"))

        # Families (hex encoded)
        families_hex = get_field(r'Families:\s*([0-9a-fA-F]+)', "")
        if families_hex:
            family_indices = parse_hex_to_int_list(families_hex)
            card.families = [CARD_FAMILIES[i] for i in family_indices if i < len(CARD_FAMILIES)]

        # Abilities (hex encoded)
        abilities_hex = get_field(r'Abilities:\s*([0-9a-fA-F]+)', "")
        if abilities_hex:
            ability_indices = parse_hex_to_int_list(abilities_hex)
            card.abilities = [INVOCATION_ABILITIES[i] for i in ability_indices if i < len(INVOCATION_ABILITIES)]

        # Conditions (may be empty or hex)
        conditions_match = re.search(r'Conditions:\s*\n?((?:\s+-\s+\d+\n?)*|[0-9a-fA-F]*)', content)
        if conditions_match:
            cond_str = conditions_match.group(1).strip()
            if cond_str and not cond_str.startswith('-'):
                cond_indices = parse_hex_to_int_list(cond_str)
                card.conditions = [CONDITIONS[i] for i in cond_indices if i < len(CONDITIONS)]

    elif card_type == "Equipment":
        # Equipment abilities
        eq_abilities_hex = get_field(r'EquipmentAbilities:\s*([0-9a-fA-F]+)', "")
        if eq_abilities_hex:
            ability_indices = parse_hex_to_int_list(eq_abilities_hex)
            card.abilities = [EQUIPMENT_ABILITIES[i] for i in ability_indices if i < len(EQUIPMENT_ABILITIES)]

    elif card_type == "Field":
        # Field family
        family_idx = int(get_field(r'family:\s*(\d+)', "0"))
        if family_idx < len(CARD_FAMILIES):
            card.field_family = CARD_FAMILIES[family_idx]

        # Field abilities
        field_abilities_hex = get_field(r'FieldAbilities:\s*([0-9a-fA-F]+)', "")
        if field_abilities_hex:
            ability_indices = parse_hex_to_int_list(field_abilities_hex)
            card.abilities = [FIELD_ABILITIES[i] for i in ability_indices if i < len(FIELD_ABILITIES)]

    elif card_type == "Effect":
        # Effect abilities
        effect_abilities_hex = get_field(r'EffectAbilities:\s*([0-9a-fA-F]+)', "")
        if effect_abilities_hex:
            ability_indices = parse_hex_to_int_list(effect_abilities_hex)
            card.abilities = [EFFECT_ABILITIES[i] for i in ability_indices if i < len(EFFECT_ABILITIES)]

    return card


def discover_all_cards(cards_dir: Path) -> Dict[str, List[CardData]]:
    """Discover all cards and group by type."""
    cards_by_type = defaultdict(list)

    for asset_file in cards_dir.glob("*.asset"):
        card = parse_asset_file(asset_file)
        if card:
            cards_by_type[card.card_type].append(card)

    # Sort each type by name
    for card_type in cards_by_type:
        cards_by_type[card_type].sort(key=lambda c: c.name)

    return dict(cards_by_type)


def analyze_synergies(cards_by_type: Dict[str, List[CardData]]) -> Dict:
    """Analyze card synergies and combinations."""
    synergies = {
        "family_synergies": defaultdict(list),  # family -> cards that boost it
        "equipment_targets": [],  # equipment that work well with specific invocations
        "field_family_map": defaultdict(list),  # field cards by family
        "dependency_chains": [],  # cards that depend on other specific cards
        "sacrifice_chains": [],  # sacrifice ability chains
        "deck_search_abilities": [],  # abilities that search the deck
        "protection_combos": [],  # protection ability combos
    }

    invocations = cards_by_type.get("Invocation", [])
    equipments = cards_by_type.get("Equipment", [])
    fields = cards_by_type.get("Field", [])
    effects = cards_by_type.get("Effect", [])

    # Analyze invocations for family boosts
    for card in invocations:
        for ability in card.abilities:
            # Family stat givers
            if "GiveAtkDef" in ability or "Win1Atk1Def" in ability:
                for fam in ["Comics", "Developer", "Fistiland", "HardCorner", "Japan", "Rpg"]:
                    if fam.lower() in ability.lower():
                        synergies["family_synergies"][fam].append({
                            "card": card.name,
                            "ability": ability
                        })

            # Deck search abilities
            if "FromDeck" in ability or "Get" in ability:
                synergies["deck_search_abilities"].append({
                    "card": card.name,
                    "ability": ability,
                    "target": ability.replace("Get", "").replace("FromDeck", "")
                })

            # Sacrifice abilities
            if "Sacrifice" in ability:
                synergies["sacrifice_chains"].append({
                    "card": card.name,
                    "ability": ability
                })

            # Protection abilities
            if "Protect" in ability or "CantBeAttack" in ability or "Survive" in ability:
                synergies["protection_combos"].append({
                    "card": card.name,
                    "ability": ability
                })

        # Dependency analysis (CantLiveWithout)
        for ability in card.abilities:
            if "CantLiveWithout" in ability:
                synergies["dependency_chains"].append({
                    "card": card.name,
                    "requires": ability.replace("CantLiveWithout", ""),
                    "ability": ability
                })

    # Field cards by family
    for card in fields:
        if card.field_family:
            synergies["field_family_map"][card.field_family].append({
                "card": card.name,
                "abilities": card.abilities
            })

    # Equipment analysis
    for eq in equipments:
        eq_info = {
            "equipment": eq.name,
            "abilities": eq.abilities,
            "best_with": []
        }

        for ability in eq.abilities:
            # Direct attack equipment works best with high ATK cards
            if "DirectAttack" in ability:
                high_atk = [c for c in invocations if c.attack >= 4]
                eq_info["best_with"].extend([c.name for c in high_atk[:5]])

            # ATK multipliers work well with already high ATK
            if "MultiplyAtk" in ability:
                high_atk = sorted(invocations, key=lambda c: c.attack, reverse=True)[:5]
                eq_info["best_with"].extend([c.name for c in high_atk])

            # DEF multipliers for high DEF cards
            if "MultiplyDef" in ability:
                high_def = sorted(invocations, key=lambda c: c.defense, reverse=True)[:5]
                eq_info["best_with"].extend([c.name for c in high_def])

        if eq_info["best_with"]:
            synergies["equipment_targets"].append(eq_info)

    return synergies


def generate_markdown_catalog(cards_by_type: Dict[str, List[CardData]],
                               synergies: Dict,
                               output_path: Path):
    """Generate a comprehensive markdown catalog."""

    md = []
    md.append("# JDG Card Game - Complete Power Catalog")
    md.append("")
    md.append("This document catalogs all abilities, powers, and synergies in the JDG Trading Card Game.")
    md.append("")
    md.append("## Table of Contents")
    md.append("")
    md.append("1. [Overview](#overview)")
    md.append("2. [Card Statistics](#card-statistics)")
    md.append("3. [Invocation Cards](#invocation-cards)")
    md.append("4. [Equipment Cards](#equipment-cards)")
    md.append("5. [Field Cards](#field-cards)")
    md.append("6. [Effect Cards](#effect-cards)")
    md.append("7. [Contre Cards](#contre-cards)")
    md.append("8. [Ability Reference](#ability-reference)")
    md.append("9. [Synergies & Combinations](#synergies--combinations)")
    md.append("10. [Testing Checklist](#testing-checklist)")
    md.append("")

    # Overview
    md.append("## Overview")
    md.append("")
    total = sum(len(cards) for cards in cards_by_type.values())
    md.append(f"**Total Cards**: {total}")
    md.append("")
    for card_type, cards in sorted(cards_by_type.items()):
        md.append(f"- **{card_type}**: {len(cards)} cards")
    md.append("")

    # Card Statistics
    md.append("## Card Statistics")
    md.append("")

    # Family distribution
    family_counts = defaultdict(int)
    for card in cards_by_type.get("Invocation", []):
        for fam in card.families:
            family_counts[fam] += 1

    md.append("### Family Distribution (Invocations)")
    md.append("")
    md.append("| Family | Count |")
    md.append("|--------|-------|")
    for fam, count in sorted(family_counts.items(), key=lambda x: -x[1]):
        md.append(f"| {fam} | {count} |")
    md.append("")

    # Invocation Cards
    md.append("## Invocation Cards")
    md.append("")

    invocations = cards_by_type.get("Invocation", [])

    # Group by family
    by_family = defaultdict(list)
    for card in invocations:
        if card.families:
            for fam in card.families:
                by_family[fam].append(card)
        else:
            by_family["None"].append(card)

    for family in sorted(by_family.keys()):
        cards = by_family[family]
        md.append(f"### {family} Family ({len(cards)} cards)")
        md.append("")
        md.append("| Card | ATK | DEF | Abilities | Conditions |")
        md.append("|------|-----|-----|-----------|------------|")
        for card in sorted(cards, key=lambda c: c.name):
            abilities_str = ", ".join(card.abilities) if card.abilities else "None"
            conditions_str = ", ".join(card.conditions) if card.conditions else "None"
            md.append(f"| {card.name} | {card.attack:.0f} | {card.defense:.0f} | {abilities_str} | {conditions_str} |")
        md.append("")

    # Equipment Cards
    md.append("## Equipment Cards")
    md.append("")
    md.append("| Card | Abilities | Description |")
    md.append("|------|-----------|-------------|")
    for card in cards_by_type.get("Equipment", []):
        abilities_str = ", ".join(card.abilities) if card.abilities else "None"
        desc = card.detailed_description[:80] + "..." if len(card.detailed_description) > 80 else card.detailed_description
        md.append(f"| {card.name} | {abilities_str} | {desc} |")
    md.append("")

    # Field Cards
    md.append("## Field Cards")
    md.append("")
    md.append("| Card | Family | Abilities | Description |")
    md.append("|------|--------|-----------|-------------|")
    for card in cards_by_type.get("Field", []):
        abilities_str = ", ".join(card.abilities) if card.abilities else "None"
        desc = card.detailed_description[:60] + "..." if len(card.detailed_description) > 60 else card.detailed_description
        md.append(f"| {card.name} | {card.field_family} | {abilities_str} | {desc} |")
    md.append("")

    # Effect Cards
    md.append("## Effect Cards")
    md.append("")
    md.append("| Card | Abilities | Description |")
    md.append("|------|-----------|-------------|")
    for card in cards_by_type.get("Effect", []):
        abilities_str = ", ".join(card.abilities) if card.abilities else "None"
        desc = card.detailed_description[:80] + "..." if len(card.detailed_description) > 80 else card.detailed_description
        md.append(f"| {card.name} | {abilities_str} | {desc} |")
    md.append("")

    # Contre Cards
    md.append("## Contre Cards")
    md.append("")
    md.append("| Card | Description |")
    md.append("|------|-------------|")
    for card in cards_by_type.get("Contre", []):
        md.append(f"| {card.name} | {card.detailed_description} |")
    md.append("")

    # Ability Reference
    md.append("## Ability Reference")
    md.append("")

    md.append("### Invocation Abilities")
    md.append("")

    # Categorize invocation abilities
    ability_categories = {
        "Draw": [a for a in INVOCATION_ABILITIES if "Draw" in a],
        "Sacrifice": [a for a in INVOCATION_ABILITIES if "Sacrifice" in a],
        "Deck Search": [a for a in INVOCATION_ABILITIES if "FromDeck" in a or "Get" in a],
        "Invoke": [a for a in INVOCATION_ABILITIES if "Invoke" in a],
        "Protection": [a for a in INVOCATION_ABILITIES if "Protect" in a or "CantBeAttack" in a or "Survive" in a],
        "Destruction": [a for a in INVOCATION_ABILITIES if "Destroy" in a or "Kill" in a],
        "Stat Boost": [a for a in INVOCATION_ABILITIES if "Win" in a or "GiveAtk" in a or "Give" in a],
        "Dependency": [a for a in INVOCATION_ABILITIES if "CantLiveWithout" in a],
        "Resurrection": [a for a in INVOCATION_ABILITIES if "ComesBack" in a or "Death" in a],
        "Control": [a for a in INVOCATION_ABILITIES if "SendAll" in a or "Skip" in a or "Copy" in a],
        "Field": [a for a in INVOCATION_ABILITIES if "Field" in a],
    }

    for category, abilities in ability_categories.items():
        if abilities:
            md.append(f"#### {category} Abilities")
            md.append("")
            for ability in abilities:
                # Find cards with this ability
                cards_with = [c.name for c in invocations if ability in c.abilities]
                cards_str = ", ".join(cards_with) if cards_with else "None"
                md.append(f"- **{ability}**: {cards_str}")
            md.append("")

    md.append("### Equipment Abilities")
    md.append("")
    for ability in EQUIPMENT_ABILITIES:
        if ability != "Default":
            cards_with = [c.name for c in cards_by_type.get("Equipment", []) if ability in c.abilities]
            cards_str = ", ".join(cards_with) if cards_with else "None"
            md.append(f"- **{ability}**: {cards_str}")
    md.append("")

    md.append("### Field Abilities")
    md.append("")
    for ability in FIELD_ABILITIES:
        if ability != "Default":
            cards_with = [c.name for c in cards_by_type.get("Field", []) if ability in c.abilities]
            cards_str = ", ".join(cards_with) if cards_with else "None"
            md.append(f"- **{ability}**: {cards_str}")
    md.append("")

    md.append("### Effect Abilities")
    md.append("")
    for ability in EFFECT_ABILITIES:
        if ability != "Default":
            cards_with = [c.name for c in cards_by_type.get("Effect", []) if ability in c.abilities]
            cards_str = ", ".join(cards_with) if cards_with else "None"
            md.append(f"- **{ability}**: {cards_str}")
    md.append("")

    # Synergies
    md.append("## Synergies & Combinations")
    md.append("")

    md.append("### Family Boost Synergies")
    md.append("")
    md.append("Cards that boost specific families:")
    md.append("")
    for family, boosters in sorted(synergies["family_synergies"].items()):
        if boosters:
            md.append(f"#### {family}")
            for boost in boosters:
                md.append(f"- **{boost['card']}**: {boost['ability']}")
            md.append("")

    md.append("### Field Card + Invocation Synergies")
    md.append("")
    for family, field_cards in sorted(synergies["field_family_map"].items()):
        invocations_in_family = [c.name for c in invocations if family in c.families]
        if field_cards and invocations_in_family:
            md.append(f"#### {family}")
            md.append(f"**Field Cards**: {', '.join([f['card'] for f in field_cards])}")
            md.append(f"**Invocations**: {', '.join(invocations_in_family[:10])}{'...' if len(invocations_in_family) > 10 else ''}")
            md.append("")

    md.append("### Equipment Combinations")
    md.append("")
    for eq_info in synergies["equipment_targets"]:
        if eq_info["best_with"]:
            md.append(f"#### {eq_info['equipment']}")
            md.append(f"**Abilities**: {', '.join(eq_info['abilities'])}")
            md.append(f"**Best With**: {', '.join(set(eq_info['best_with'][:5]))}")
            md.append("")

    md.append("### Card Dependencies")
    md.append("")
    md.append("Cards that require other cards to stay alive:")
    md.append("")
    for dep in synergies["dependency_chains"]:
        md.append(f"- **{dep['card']}** requires **{dep['requires']}**")
    md.append("")

    md.append("### Sacrifice Chains")
    md.append("")
    md.append("Cards with sacrifice abilities:")
    md.append("")
    for sac in synergies["sacrifice_chains"]:
        md.append(f"- **{sac['card']}**: {sac['ability']}")
    md.append("")

    md.append("### Deck Search Network")
    md.append("")
    md.append("Cards that can search for other cards:")
    md.append("")
    for search in synergies["deck_search_abilities"]:
        md.append(f"- **{search['card']}** -> {search['target']}")
    md.append("")

    md.append("### Protection Abilities")
    md.append("")
    for prot in synergies["protection_combos"]:
        md.append(f"- **{prot['card']}**: {prot['ability']}")
    md.append("")

    # Testing Checklist
    md.append("## Testing Checklist")
    md.append("")
    md.append("### Invocation Abilities to Test")
    md.append("")
    for ability in INVOCATION_ABILITIES:
        if ability != "Default":
            md.append(f"- [ ] {ability}")
    md.append("")

    md.append("### Equipment Abilities to Test")
    md.append("")
    for ability in EQUIPMENT_ABILITIES:
        if ability != "Default":
            md.append(f"- [ ] {ability}")
    md.append("")

    md.append("### Field Abilities to Test")
    md.append("")
    for ability in FIELD_ABILITIES:
        if ability != "Default":
            md.append(f"- [ ] {ability}")
    md.append("")

    md.append("### Effect Abilities to Test")
    md.append("")
    for ability in EFFECT_ABILITIES:
        if ability != "Default":
            md.append(f"- [ ] {ability}")
    md.append("")

    md.append("### Conditions to Test")
    md.append("")
    for condition in CONDITIONS:
        md.append(f"- [ ] {condition}")
    md.append("")

    md.append("### Key Combinations to Test")
    md.append("")
    md.append("#### Equipment + Invocation")
    for eq_info in synergies["equipment_targets"][:10]:
        if eq_info["best_with"]:
            best = eq_info["best_with"][0] if eq_info["best_with"] else "Any"
            md.append(f"- [ ] {eq_info['equipment']} + {best}")
    md.append("")

    md.append("#### Field + Family")
    for family, field_cards in synergies["field_family_map"].items():
        if field_cards:
            md.append(f"- [ ] {field_cards[0]['card']} + {family} invocations")
    md.append("")

    md.append("#### Dependency Chains")
    for dep in synergies["dependency_chains"][:10]:
        md.append(f"- [ ] {dep['card']} without {dep['requires']}")
    md.append("")

    # Write to file
    output_path.write_text("\n".join(md), encoding='utf-8')
    print(f"Catalog written to: {output_path}")
    return len(md)


def main():
    # Find cards directory
    script_dir = Path(__file__).parent
    project_root = script_dir.parent
    cards_dir = project_root / "JDG Mobile Game" / "Assets" / "Resources" / "Cards"

    if not cards_dir.exists():
        print(f"Cards directory not found: {cards_dir}")
        return

    print(f"Scanning cards in: {cards_dir}")

    # Discover all cards
    cards_by_type = discover_all_cards(cards_dir)

    # Print summary
    print("\n=== Card Discovery Summary ===")
    total = 0
    for card_type, cards in sorted(cards_by_type.items()):
        print(f"{card_type}: {len(cards)} cards")
        total += len(cards)
    print(f"Total: {total} cards")

    # Analyze synergies
    print("\nAnalyzing synergies...")
    synergies = analyze_synergies(cards_by_type)

    # Generate catalog
    output_path = project_root / "JDG Mobile Game" / "docs" / "CARD_POWER_CATALOG.md"
    output_path.parent.mkdir(parents=True, exist_ok=True)

    lines = generate_markdown_catalog(cards_by_type, synergies, output_path)
    print(f"\nGenerated catalog with {lines} lines")

    # Also save raw data as JSON for further analysis
    json_path = project_root / "JDG Mobile Game" / "docs" / "card_data.json"

    # Convert to serializable format
    export_data = {
        "cards_by_type": {
            ct: [
                {
                    "name": c.name,
                    "type": c.card_type,
                    "attack": c.attack,
                    "defense": c.defense,
                    "families": c.families,
                    "abilities": c.abilities,
                    "conditions": c.conditions,
                    "field_family": c.field_family,
                    "description": c.description,
                    "is_collector": c.is_collector
                }
                for c in cards
            ]
            for ct, cards in cards_by_type.items()
        },
        "synergies": {
            "family_synergies": dict(synergies["family_synergies"]),
            "field_family_map": dict(synergies["field_family_map"]),
            "equipment_targets": synergies["equipment_targets"],
            "dependency_chains": synergies["dependency_chains"],
            "sacrifice_chains": synergies["sacrifice_chains"],
            "deck_search_abilities": synergies["deck_search_abilities"],
            "protection_combos": synergies["protection_combos"]
        },
        "ability_enums": {
            "invocation": INVOCATION_ABILITIES,
            "equipment": EQUIPMENT_ABILITIES,
            "field": FIELD_ABILITIES,
            "effect": EFFECT_ABILITIES,
            "conditions": CONDITIONS
        }
    }

    with open(json_path, 'w', encoding='utf-8') as f:
        json.dump(export_data, f, indent=2, ensure_ascii=False)
    print(f"Raw data saved to: {json_path}")


if __name__ == "__main__":
    main()
