---
name: writing-plans
description: Use when you have a spec or requirements for a multi-step task, before touching code
---

# Writing Plans

## Overview

Write comprehensive implementation plans assuming the engineer has zero context for our codebase and questionable taste. Document everything they need to know: which files to touch for each task, code, testing, docs they might need to check, how to test it. Give them the whole plan as bite-sized tasks. DRY. YAGNI. TDD. Frequent commits.

Assume they are a skilled developer, but know almost nothing about our toolset or problem domain. Assume they don't know good test design very well.

**Announce at start:** "I'm using the writing-plans skill to create the implementation plan."

**Context:** This should be run in a dedicated worktree (created by brainstorming skill).

**Save plans to:** `docs/plans/YYYY-MM-DD-<feature-name>.md`

## Bite-Sized Task Granularity

**Each step is one action (2-5 minutes):**
- "Write the failing test" - step
- "Run it to make sure it fails" - step
- "Implement the minimal code to make the test pass" - step
- "Run the tests and make sure they pass" - step
- "Commit" - step

## Plan Document Header

**Every plan MUST start with this header:**

```markdown
# [Feature Name] Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** [One sentence describing what this builds]

**Architecture:** [2-3 sentences about approach]

**Tech Stack:** JDK17 / Spring Boot 3.2.4 / MariaDB / Spring JPA / Activiti BPMN 8.0 / JUnit 5 / Mockito / H2 (Test) / Cucumber

---
```

## Task Structure

```markdown
### Task N: [Component Name]

**Files:**
- Create: `src/main/java/com/example/path/File.java`
- Modify: `src/main/java/com/example/path/Existing.java:123-145`
- Test: `src/test/java/com/example/path/FileTest.java`

**Step 1: Write the failing test**

```java
@Test
void specificBehavior_whenCalled_returnsExpected() {
    var result = sut.function(input);
    assertThat(result).isEqualTo(expected);
}
```

**Step 2: Run test to verify it fails**

Run: `mvn test -Dtest=FileTest#specificBehavior_whenCalled_returnsExpected`
Expected: FAIL with "cannot find symbol"

**Step 3: Write minimal implementation**

```java
public Result function(Input input) {
    return expected;
}
```

**Step 4: Run test to verify it passes**

Run: `mvn test -Dtest=FileTest#specificBehavior_whenCalled_returnsExpected`
Expected: PASS

**Step 5: Commit**

```bash
git add src/test/java/com/example/path/FileTest.java src/main/java/com/example/path/File.java
git commit -m "feat: add specific feature"
```
```

## Remember
- Exact file paths always
- Complete code in plan (not "add validation")
- Exact commands with expected output
- Reference relevant skills with @ syntax
- DRY, YAGNI, TDD, frequent commits

## Execution Handoff

After saving the plan, offer execution choice:

**"Plan complete and saved to `docs/plans/<filename>.md`. Two execution options:**

**1. Subagent-Driven (this session)** - I dispatch fresh subagent per task, review between tasks, fast iteration

**2. Parallel Session (separate)** - Open new session with executing-plans, batch execution with checkpoints

**Which approach?"**

**If Subagent-Driven chosen:**
- **REQUIRED SUB-SKILL:** Use superpowers:subagent-driven-development
- Stay in this session
- Fresh subagent per task + code review

**If Parallel Session chosen:**
- Guide them to open new session in worktree
- **REQUIRED SUB-SKILL:** New session uses superpowers:executing-plans
