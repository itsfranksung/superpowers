---
name: brainstorming
description: "You MUST use this before any creative work - creating features, building components, adding functionality, or modifying behavior. Explores user intent, requirements and design before implementation."
---

# Brainstorming Ideas Into Designs

## Overview

Help turn ideas into fully formed designs and specs through natural collaborative dialogue.

Start by understanding the current project context, then ask questions one at a time to refine the idea. Once you understand what you're building, present the design as Gherkin feature files in small sections, checking after each section whether it looks right so far.

## The Process

**Understanding the idea:**
- Check out the current project state first (files, docs, recent commits)
- Ask questions one at a time to refine the idea
- Prefer multiple choice questions when possible, but open-ended is fine too
- Only one question per message - if a topic needs more exploration, break it into multiple questions
- Focus on understanding: purpose, constraints, success criteria

**Exploring approaches:**
- Propose 2-3 different approaches with trade-offs
- Present options conversationally with your recommendation and reasoning
- Lead with your recommended option and explain why

**Presenting the design:**
- Once you believe you understand what you're building, present the design as Gherkin scenarios
- Break it into Feature sections, presenting 2-3 Scenarios at a time
- Ask after each section whether it looks right so far
- Cover: Feature description, Background, Scenarios, error handling, edge cases
- Be ready to go back and clarify if something doesn't make sense

## Gherkin Output Format

All specifications MUST be written in Gherkin language following this structure:

```gherkin
# language: zh-TW (or en, depending on project language)
Feature: <Feature Name>
  <Feature description - explains the business value and context>

  Background:
    Given <common preconditions shared by all scenarios>

  Scenario: <Scenario Name>
    Given <initial context>
    When <action taken>
    Then <expected outcome>
    And <additional outcome if needed>

  Scenario Outline: <Parameterized Scenario Name>
    Given <context with <parameter>>
    When <action with <parameter>>
    Then <expected outcome with <parameter>>

    Examples:
      | parameter |
      | value1    |
      | value2    |
```

**Gherkin Best Practices:**
- Use `Feature` to describe business capability
- Use `Background` for shared preconditions across scenarios
- Use `Scenario` for specific test cases
- Use `Scenario Outline` with `Examples` for parameterized tests
- Keep steps declarative (WHAT, not HOW)
- Use `Given` for preconditions, `When` for actions, `Then` for outcomes
- Use `And`/`But` to chain steps of the same type

## After the Design

**Documentation:**
- Write the validated specifications to `docs/specs/YYYY-MM-DD-<topic>.feature`
- Group related features into the same `.feature` file or split into multiple files by domain
- Use elements-of-style:writing-clearly-and-concisely skill if available
- Commit the feature files to git

**Implementation (if continuing):**
- Ask: "Ready to set up for implementation?"
- Use superpowers:using-git-worktrees to create isolated workspace
- Use superpowers:writing-plans to create detailed implementation plan

## Key Principles

- **One question at a time** - Don't overwhelm with multiple questions
- **Multiple choice preferred** - Easier to answer than open-ended when possible
- **YAGNI ruthlessly** - Remove unnecessary features from all designs
- **Explore alternatives** - Always propose 2-3 approaches before settling
- **Incremental validation** - Present design in sections, validate each
- **Be flexible** - Go back and clarify when something doesn't make sense
