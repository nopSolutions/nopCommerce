# AI and LLM Usage Documentation

## Purpose of This Document

This document transparently describes how AI/LLM tools (specifically Claude Code) were used throughout this assignment, in accordance with academic integrity principles and best practices for AI-assisted software development.

## Overview of AI Usage

AI was used as an **agentic partner** throughout this assignment and not as a replacement for understanding. As a tool it helped with:
- Explore and analyse nopCommerce's existing architecture
- Generate documentation drafts that we read, validate, and refine
- Suggest architectural approaches and identify tradeoffs
- Help implement the plugin, worker, and simulators after the approach is approved
- Review and critique architectural decisions

**Critical principle**: All AI-generated content and decisions were reviewed, questioned, and understood before being accepted. The assignment is evaluated on architectural judgment — that judgment is ours.

## Specific Usage Patterns

### 1. Architecture Analysis and Documentation
**How**: Claude was asked to analyse the nopCommerce codebase, identify extension points relevant to Scenario C, and generate documentation drafts (current-state analysis, ADRs, diagrams).

**Our role**:
- Read all generated documentation carefully
- Verify accuracy against the actual codebase
- Revise or reject sections that were incorrect or superficial
- Ensure we understood every architectural claim made

**Why this approach**: AI can produce a strong first draft quickly; we validate and refine it with our own understanding of the code and the scenario.

### 2. Architectural Decisions with Plan Mode
**How**: Claude operates in plan mode where it:
1. Proposes an approach and explains the tradeoffs
2. Waits for our approval before making any changes
3. We evaluate, question, and approve or redirect
4. Only after approval does it implement

**Our role**:
- Critically evaluate every proposed decision
- Ask "why this and not that" — especially for ADRs with rejected alternatives
- Reject proposals that do not align with the scenario's quality attributes
- Ensure we can defend every decision independently

**Why this approach**: This prevents blind acceptance of AI suggestions and ensures we are making informed, defensible architectural choices.

### 3. Providing Context
**How**: We provide Claude with:
- Assignment PDFs and requirements
- Repository structure and key source files
- The chosen scenario (Scenario C — Omnichannel Commerce Core) and its quality attribute drivers
- Architecture diagrams and domain boundary decisions

**Our role**: Curating what context is relevant and ensuring Claude has accurate, assignment-specific information.

**Why this approach**: AI works best as a collaborative partner when it has full context of requirements and constraints — without that context it produces generic output.

### 4. Iterative Questioning and Validation
**How**: Throughout the work:
- We ask "why did you choose this approach?"
- We challenge design decisions: "What is the tradeoff of outbox vs. direct call?"
- We question scope: "Is this the minimum change that satisfies the scenario?"
- We test all implementation changes locally
- We verify the demo scenarios end to end before presenting

**Our role**: Critical thinking, validation, and ensuring we can defend every decision in the live presentation.

**Why this approach**: AI can propose solutions, but only we can validate they meet the assignment's requirement for genuine architectural judgment under pressure.

## Conclusion

This assignment is about **architectural judgment**: understanding tradeoffs, making justified decisions, and demonstrating a system that behaves credibly under pressure. AI helped us work faster, but every architectural decision, ADR, and demo scenario reflects our own analysis and understanding.

AI is a tool. We are the architects.

---

**Date**: May 2026
**Assignment**: Architectural Evolution of nopCommerce — Assignment 2 (Group, 50%)
**Scenario**: Scenario C — Omnichannel Commerce Core
**Course**: Software Architectures — Master in Informatics Engineering
