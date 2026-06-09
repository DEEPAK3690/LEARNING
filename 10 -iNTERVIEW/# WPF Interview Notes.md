# WPF Interview Notes

## Grid

**Grid** provides row- and column-based layout management.

- It gives precise control over positioning, alignment, and spacing of controls.
- Use **StackPanel** or **WrapPanel** for sequential layouts.
- Use **Grid** when structured alignment and complex layouts are required.

---

## Data Binding

**Data Binding** connects a ViewModel or C# object to the UI.

- When the source property changes, the UI can update automatically.
- Data binding typically requires a **DataContext**.

### Binding Modes

#### OneWay Binding
- Updates UI when source changes.
- Source → UI

#### TwoWay Binding
- Updates both source and UI.
- Source ↔ UI

---

## INotifyPropertyChanged

`INotifyPropertyChanged` is used to notify the UI when a ViewModel property changes.

- Required for **Source → UI** updates.
- Raises the `PropertyChanged` event.
- Without it, changes made in the ViewModel will not automatically appear in the UI.

```csharp
public event PropertyChangedEventHandler PropertyChanged;
```

### Important Interview Point

- UI → Source updates can still happen through TwoWay Binding even without `INotifyPropertyChanged`.
- But Source → UI updates require `INotifyPropertyChanged`.

---

## ICommand

`ICommand` is an interface used to handle UI actions in the ViewModel.

### Benefits

- Supports MVVM architecture.
- Removes button click logic from code-behind.
- Improves testability and separation of concerns.

### Example

```xml
<Button Command="{Binding SaveCommand}" />
```

---

## IValueConverter

`IValueConverter` is used when the data type in the ViewModel differs from what the UI expects.

### Common Examples

- Boolean → Visibility
- Enum → String
- DateTime → Formatted Text

It converts data between the source and target during binding.

---

## Resources

Resources are reusable objects stored in XAML.

### Examples

- Styles
- Brushes
- Templates
- Converters

### Benefits

- Reusability
- Maintainability
- Consistent UI

---

## ControlTemplate

A **ControlTemplate** defines the visual appearance and structure of a control.

It changes **how a control looks** without changing its behavior.

### Example

- Redesigning a Button completely while keeping Button functionality.

### Interview Answer

> ControlTemplate is used to customize the visual structure and appearance of a control while preserving its functionality.

---

## DataTemplate

A **DataTemplate** defines how a data object should be displayed in the UI.

Think of it as a **display template for data**.

### Used When

- Displaying ViewModels
- Complex objects
- Collection items

### Example

```xml
<ListBox ItemsSource="{Binding Employees}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" />
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### Interview Difference

#### ControlTemplate
- Defines appearance of a control.

#### DataTemplate
- Defines appearance of data.

---

## Triggers

### DataTrigger

A **DataTrigger** monitors data from the ViewModel or Model through Binding.

Used when UI should react to business data.

### Example

```xml
<DataTrigger Binding="{Binding IsActive}" Value="True">
```

### Property Trigger

A **Property Trigger** monitors a control's own property.

### Common Examples

- IsMouseOver
- IsFocused
- IsPressed

### Example

```xml
<Trigger Property="IsMouseOver" Value="True">
```

### Interview Difference

#### DataTrigger
- Watches ViewModel/Data properties.

#### Property Trigger
- Watches UI control properties.

---

## x:Name

`x:Name` assigns a unique name to a XAML element.

It allows access to that control from code-behind.

### Example

```xml
<Button x:Name="btnSave"/>
```

---

## XAML Basics

XAML consists of:

- Element
- Property
- Value

### Example

```xml
<Button Content="Save"/>
```

| Part | Value |
|--------|--------|
| Element | Button |
| Property | Content |
| Value | Save |

---

## Dependency Property

A **Dependency Property** is a special property managed by the WPF Property System.

### Features Supported

- Data Binding
- Styles
- Templates
- Triggers
- Animations
- Default Values
- Property Value Inheritance

### Why Not Normal CLR Properties?

Normal CLR properties only store values.

Dependency Properties provide additional WPF functionality and participate in the WPF property system.

### Interview Answer

> Dependency Properties are special properties managed by WPF that enable advanced features such as data binding, styling, animations, triggers, and property value inheritance.

---

## Async/Await in WPF

Use `async` and `await` in ViewModel commands to:

- Keep the UI responsive.
- Avoid UI freezes.
- Write asynchronous code in a readable manner.

### Example

```csharp
public async Task LoadDataAsync()
{
    await service.GetDataAsync();
}
```

### Interview Answer

> I use async/await in ViewModel commands to perform long-running operations without blocking the UI thread and to keep the application responsive.

---

## Dispatcher

WPF controls can only be accessed from the UI thread.

If work is performed on a background thread and the UI must be updated, use:

```csharp
Dispatcher.Invoke(() =>
{
    // Update UI
});
```

or

```csharp
Dispatcher.BeginInvoke(() =>
{
    // Update UI
});
```

to marshal execution back to the UI thread.

### Interview Answer

> Since WPF follows thread affinity, UI elements can only be accessed from the UI thread. If a background thread needs to update the UI, I use Dispatcher.Invoke or Dispatcher.BeginInvoke to execute that code on the UI thread.

---

# Quick Interview Differences

| Concept | Purpose |
|----------|----------|
| Grid | Structured row/column layout |
| StackPanel | Sequential layout |
| WrapPanel | Sequential layout with wrapping |
| OneWay Binding | Source → UI |
| TwoWay Binding | Source ↔ UI |
| ControlTemplate | Changes control appearance |
| DataTemplate | Changes data appearance |
| Property Trigger | Watches control property |
| DataTrigger | Watches ViewModel property |
| ICommand | Handles UI actions in ViewModel |
| INotifyPropertyChanged | Notifies UI of property changes |
| Dependency Property | Enables WPF features like binding and styling |
| Dispatcher | Updates UI from background thread |

---

# One-Line Interview Revision

- **Grid** → Best for row/column-based layouts.
- **StackPanel** → Arranges controls sequentially.
- **WrapPanel** → Sequential layout with automatic wrapping.
- **Data Binding** → Connects ViewModel data to UI.
- **OneWay Binding** → Source updates UI.
- **TwoWay Binding** → Source and UI update each other.
- **INotifyPropertyChanged** → Notifies UI when data changes.
- **ICommand** → Moves UI actions to the ViewModel.
- **IValueConverter** → Converts values between ViewModel and UI.
- **Resource** → Reusable XAML objects.
- **ControlTemplate** → Defines control appearance.
- **DataTemplate** → Defines how data is displayed.
- **DataTrigger** → Reacts to ViewModel data changes.
- **Property Trigger** → Reacts to control property changes.
- **x:Name** → Gives a unique name to a XAML element.
- **Dependency Property** → Supports binding, styling, triggers, and animations.
- **async/await** → Keeps UI responsive during long-running operations.
- **Dispatcher** → Updates UI from background threads.