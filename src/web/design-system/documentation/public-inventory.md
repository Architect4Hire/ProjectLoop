# Public design-system inventory

Traceability: PLDS-001.

This checked-in snapshot maps every TypeScript leaf module reachable from
`public-api.ts` through the category barrels to its implementation and the guide
assigned by `catalog.json`. It contains 70 public modules. Regenerate the
snapshot from those sources when a public barrel or catalog mapping changes;
do not infer missing items from this table alone.

| Category | Public item | Implementation | Guide |
| --- | --- | --- | --- |
| Tokens | Borders | [tokens/borders](../tokens/borders.ts) | [elevation-and-layers.md](elevation-and-layers.md) |
| Tokens | Elevation | [tokens/elevation](../tokens/elevation.ts) | [elevation-and-layers.md](elevation-and-layers.md) |
| Tokens | Layers | [tokens/layers](../tokens/layers.ts) | [elevation-and-layers.md](elevation-and-layers.md) |
| Tokens | Motion | [tokens/motion](../tokens/motion.ts) | [motion.md](motion.md) |
| Tokens | Radius | [tokens/radius](../tokens/radius.ts) | [elevation-and-layers.md](elevation-and-layers.md) |
| Tokens | Semantic colors | [tokens/semantic-colors](../tokens/semantic-colors.ts) | [semantic-colors.md](semantic-colors.md) |
| Tokens | Sizing | [tokens/sizing](../tokens/sizing.ts) | [spacing-and-sizing.md](spacing-and-sizing.md) |
| Tokens | Spacing | [tokens/spacing](../tokens/spacing.ts) | [spacing-and-sizing.md](spacing-and-sizing.md) |
| Tokens | Typography | [tokens/typography](../tokens/typography.ts) | [typography.md](typography.md) |
| Foundations | Appearance | [foundations/appearance.service](../foundations/appearance.service.ts) | [appearance.md](appearance.md) |
| Primitives | Badge | [primitives/badge/badge.component](../primitives/badge/badge.component.ts) | [badges-and-status.md](badges-and-status.md) |
| Primitives | Button | [primitives/button/button.component](../primitives/button/button.component.ts) | [button.md](button.md) |
| Primitives | Checkbox | [primitives/checkbox/checkbox.component](../primitives/checkbox/checkbox.component.ts) | [checkboxes-and-radios.md](checkboxes-and-radios.md) |
| Primitives | Dialog | [primitives/dialog/dialog.component](../primitives/dialog/dialog.component.ts) | [dialog.md](dialog.md) |
| Primitives | Dialog initial focus | [primitives/dialog/dialog-initial-focus.directive](../primitives/dialog/dialog-initial-focus.directive.ts) | [dialog.md](dialog.md) |
| Primitives | Drawer | [primitives/drawer/drawer.component](../primitives/drawer/drawer.component.ts) | [drawer.md](drawer.md) |
| Primitives | Drawer initial focus | [primitives/drawer/drawer-initial-focus.directive](../primitives/drawer/drawer-initial-focus.directive.ts) | [drawer.md](drawer.md) |
| Primitives | Input | [primitives/input/input.component](../primitives/input/input.component.ts) | [input.md](input.md) |
| Primitives | Radio group | [primitives/radio-group/radio-group.component](../primitives/radio-group/radio-group.component.ts) | [checkboxes-and-radios.md](checkboxes-and-radios.md) |
| Primitives | Separator | [primitives/separator/separator.component](../primitives/separator/separator.component.ts) | [surfaces-and-separators.md](surfaces-and-separators.md) |
| Primitives | Select | [primitives/select/select.component](../primitives/select/select.component.ts) | [select.md](select.md) |
| Primitives | Surface | [primitives/surface/surface.component](../primitives/surface/surface.component.ts) | [surfaces-and-separators.md](surfaces-and-separators.md) |
| Primitives | Tab panel | [primitives/tabs/tab-panel.directive](../primitives/tabs/tab-panel.directive.ts) | [tabs.md](tabs.md) |
| Primitives | Tabs | [primitives/tabs/tabs.component](../primitives/tabs/tabs.component.ts) | [tabs.md](tabs.md) |
| Primitives | Textarea | [primitives/textarea/textarea.component](../primitives/textarea/textarea.component.ts) | [textarea.md](textarea.md) |
| Primitives | Tooltip | [primitives/tooltip/tooltip.component](../primitives/tooltip/tooltip.component.ts) | [tooltip.md](tooltip.md) |
| Primitives | Tooltip trigger | [primitives/tooltip/tooltip-trigger.directive](../primitives/tooltip/tooltip-trigger.directive.ts) | [tooltip.md](tooltip.md) |
| Components | Alert banner | [components/alert-banner/alert-banner.component](../components/alert-banner/alert-banner.component.ts) | [alert-banner.md](alert-banner.md) |
| Components | Citation chip | [components/citation-chip/citation-chip.component](../components/citation-chip/citation-chip.component.ts) | [citation-chip.md](citation-chip.md) |
| Components | Data table | [components/data-table/data-table.component](../components/data-table/data-table.component.ts) | [data-table.md](data-table.md) |
| Components | File picker | [components/file-picker/file-picker.component](../components/file-picker/file-picker.component.ts) | [file-picker.md](file-picker.md) |
| Components | Notification service | [components/notification/notification.service](../components/notification/notification.service.ts) | [notifications.md](notifications.md) |
| Components | Notification viewport | [components/notification/notification-viewport.component](../components/notification/notification-viewport.component.ts) | [notifications.md](notifications.md) |
| Components | Stepper | [components/stepper/stepper.component](../components/stepper/stepper.component.ts) | [stepper.md](stepper.md) |
| Patterns | Activity stream | [patterns/activity-stream/activity-stream.component](../patterns/activity-stream/activity-stream.component.ts) | [activity-stream.md](activity-stream.md) |
| Patterns | Activity stream details | [patterns/activity-stream/activity-stream-details.directive](../patterns/activity-stream/activity-stream-details.directive.ts) | [activity-stream.md](activity-stream.md) |
| Patterns | AI confidence | [patterns/ai-confidence/ai-confidence.component](../patterns/ai-confidence/ai-confidence.component.ts) | [ai-confidence.md](ai-confidence.md) |
| Patterns | AI content | [patterns/ai-content/ai-content.component](../patterns/ai-content/ai-content.component.ts) | [ai-content.md](ai-content.md) |
| Patterns | AI generation progress | [patterns/ai-generation-progress/ai-generation-progress.component](../patterns/ai-generation-progress/ai-generation-progress.component.ts) | [ai-generation-progress.md](ai-generation-progress.md) |
| Patterns | AI failure | [patterns/ai-failure/ai-failure.component](../patterns/ai-failure/ai-failure.component.ts) | [ai-failure.md](ai-failure.md) |
| Patterns | Command palette | [patterns/command-palette/command-palette.component](../patterns/command-palette/command-palette.component.ts) | [command-palette.md](command-palette.md) |
| Patterns | Filter action bar | [patterns/filter-action-bar/filter-action-bar.component](../patterns/filter-action-bar/filter-action-bar.component.ts) | [filter-action-bar.md](filter-action-bar.md) |
| Patterns | Form section | [patterns/form-section/form-section.component](../patterns/form-section/form-section.component.ts) | [form-section.md](form-section.md) |
| Patterns | Master detail | [patterns/master-detail/master-detail.component](../patterns/master-detail/master-detail.component.ts) | [master-detail.md](master-detail.md) |
| Patterns | Master detail trigger | [patterns/master-detail/master-detail-trigger.directive](../patterns/master-detail/master-detail-trigger.directive.ts) | [master-detail.md](master-detail.md) |
| Patterns | Review approval | [patterns/review-approval/review-approval.component](../patterns/review-approval/review-approval.component.ts) | [review-approval.md](review-approval.md) |
| Patterns | Search result details | [patterns/search-results/search-result-details.directive](../patterns/search-results/search-result-details.directive.ts) | [search-results.md](search-results.md) |
| Patterns | Search results | [patterns/search-results/search-results.component](../patterns/search-results/search-results.component.ts) | [search-results.md](search-results.md) |
| Patterns | Split view | [patterns/split-view/split-view.component](../patterns/split-view/split-view.component.ts) | [split-view.md](split-view.md) |
| Patterns | Source preview | [patterns/source-preview/source-preview.component](../patterns/source-preview/source-preview.component.ts) | [source-preview.md](source-preview.md) |
| Patterns | State feedback | [patterns/state-feedback/state-feedback.component](../patterns/state-feedback/state-feedback.component.ts) | [state-feedback.md](state-feedback.md) |
| Patterns | State feedback details | [patterns/state-feedback/state-feedback-details.component](../patterns/state-feedback/state-feedback-details.component.ts) | [state-feedback.md](state-feedback.md) |
| Patterns | Suggested change | [patterns/suggested-change/suggested-change.component](../patterns/suggested-change/suggested-change.component.ts) | [suggested-change.md](suggested-change.md) |
| Patterns | Version comparison | [patterns/version-comparison/version-comparison.component](../patterns/version-comparison/version-comparison.component.ts) | [version-comparison.md](version-comparison.md) |
| Recipes | ADR summary | [recipes/adr-summary/adr-summary.component](../recipes/adr-summary/adr-summary.component.ts) | [adr-summary.md](adr-summary.md) |
| Recipes | AI generation drawer | [recipes/ai-generation-drawer/ai-generation-drawer.component](../recipes/ai-generation-drawer/ai-generation-drawer.component.ts) | [ai-generation-drawer.md](ai-generation-drawer.md) |
| Recipes | Approval actions | [recipes/approval-actions/approval-actions.component](../recipes/approval-actions/approval-actions.component.ts) | [approval-actions.md](approval-actions.md) |
| Recipes | Decision comparison | [recipes/decision-comparison/decision-comparison.component](../recipes/decision-comparison/decision-comparison.component.ts) | [decision-comparison.md](decision-comparison.md) |
| Recipes | Document section editor | [recipes/document-section-editor/document-section-editor.component](../recipes/document-section-editor/document-section-editor.component.ts) | [document-section-editor.md](document-section-editor.md) |
| Recipes | Engagement header | [recipes/engagement-header/engagement-header.component](../recipes/engagement-header/engagement-header.component.ts) | [engagement-header.md](engagement-header.md) |
| Recipes | Knowledge result | [recipes/knowledge-result/knowledge-result.component](../recipes/knowledge-result/knowledge-result.component.ts) | [knowledge-result.md](knowledge-result.md) |
| Recipes | Phase navigation | [recipes/phase-navigation/phase-navigation.component](../recipes/phase-navigation/phase-navigation.component.ts) | [phase-navigation.md](phase-navigation.md) |
| Recipes | RAID register | [recipes/raid-register/raid-register.component](../recipes/raid-register/raid-register.component.ts) | [raid-register.md](raid-register.md) |
| Recipes | Requirement row | [recipes/requirement-row/requirement-row.component](../recipes/requirement-row/requirement-row.component.ts) | [requirement-row.md](requirement-row.md) |
| Recipes | Source citations | [recipes/source-citations/source-citations.component](../recipes/source-citations/source-citations.component.ts) | [source-citations.md](source-citations.md) |
| Recipes | Workbench shell | [recipes/workbench-shell/workbench-shell.component](../recipes/workbench-shell/workbench-shell.component.ts) | [workbench-shell.md](workbench-shell.md) |
| Layouts | Structured editor | [layouts/structured-editor/structured-editor.component](../layouts/structured-editor/structured-editor.component.ts) | [structured-editor.md](structured-editor.md) |
| Layouts | Structured editor section | [layouts/structured-editor/structured-editor-section.directive](../layouts/structured-editor/structured-editor-section.directive.ts) | [structured-editor.md](structured-editor.md) |
| Components | Icon | [icons/icon.component](../icons/icon.component.ts) | [icons.md](icons.md) |
| Components | Icon names | [icons/internal/icon-paths](../icons/internal/icon-paths.ts) | [icons.md](icons.md) |
