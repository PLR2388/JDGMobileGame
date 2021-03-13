# Invocation Card

## Start Effect

Theses effects happen when a player chooses to put a Invocation card on the field.

### Get Specific Card

This key needs a card name as a value. This key also needs to be followed by Get Card Source key.

### Get Specific Type Card

This key needs a type name as value. Type can take the following value : 
* field
* equipment
* invocation
* effect
* contre

This key also needs to be followed by Get Card Source key.

### Get Card Family

This key needs a family name as value. Family can take the following value : 
* fistiland
* développeur
* humain
* hardcorner
* comics
* spatial
* incarnation
* police
* monstre
* japon
* sorcier
* rpg

 This key also needs to be followed by Get Card Source key.

### Get Card Source

This key can take the value : deck or trash. This is where we're going to search for the previous cards name.

### Remove All Invocation Cards

Remove all invocation cards on the field except the one in parameter.

### Invoke Specific Card

This key need a Invocation card name as a value. It will invoke directly the following card if there's enough space otherwise it will go into the player's hand.

### Put Field

Put a field card if there's a space otherwise it goes into the player's hand.

### Destroy Field

Destroy the current player field.

### Divide 2 ATK

Divide the current attack card by 2.

### Divide 2 DEF

Divide the current defense card by 2.

### Send To Death

A invocation card on the opponent ground can be send to the yellow trash.

### Draw X Cards

Draw a certain number of cards from the player's deck.

### Condition

Give the choice to the player to use the start Effect with a drawback. Here's a list of the current conditions :

* skipAttack

