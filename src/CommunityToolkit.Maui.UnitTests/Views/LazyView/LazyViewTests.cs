﻿using CommunityToolkit.Maui.Views;
using Xunit;

namespace CommunityToolkit.Maui.UnitTests.Views;

public class LazyViewTests : BaseViewTest
{
	[Fact]
	public void CheckHasLoadedFalseAfterConstruction()
	{
		var lazyView = new LazyView<Button>();

		Assert.False(lazyView.HasLazyViewLoaded);
	}

	[Fact(Timeout = (int)TestDuration.Short)]
	public async Task LoadViewAsync_CancellationTokenExpired()
	{
		var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
		var lazyView = new LazyView<Button>();

		// Ensure CancellationToken Expired
		await Task.Delay(100, TestContext.Current.CancellationToken);

		await Assert.ThrowsAsync<OperationCanceledException>(async () => await lazyView.LoadViewAsync(cts.Token));
	}

	[Fact(Timeout = (int)TestDuration.Short)]
	public async Task LoadViewAsync_CancellationTokenCanceled()
	{
		var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
		var lazyView = new LazyView<Button>();

		// Ensure CancellationToken Expired
		await cts.CancelAsync();

		await Assert.ThrowsAsync<OperationCanceledException>(async () => await lazyView.LoadViewAsync(cts.Token));
	}

	[Fact(Timeout = (int)TestDuration.Short)]
	public async Task CheckHasLoadedTrueAfterCallLoadView()
	{
		var lazyView = new LazyView<Button>();

		await lazyView.LoadViewAsync(TestContext.Current.CancellationToken);

		Assert.True(lazyView.HasLazyViewLoaded);
	}

	[Fact(Timeout = (int)TestDuration.Short)]
	public async Task CheckContentSetAfterLoadView()
	{
		var lazyView = new LazyView<Button>();

		await lazyView.LoadViewAsync(TestContext.Current.CancellationToken);

		Assert.True(lazyView.HasLazyViewLoaded);
		Assert.IsType<Button>(lazyView.Content, exactMatch: false);
	}

	[Fact(Timeout = (int)TestDuration.Short)]
	public async Task CheckBindingContextIsPassedToCreatedView()
	{
		var lazyView = new LazyView<Button>();
		var bindingContext = new object();
		lazyView.BindingContext = bindingContext;

		await lazyView.LoadViewAsync(TestContext.Current.CancellationToken);

		Assert.True(lazyView.HasLazyViewLoaded);
		Assert.IsType<Button>(lazyView.Content, exactMatch: false);
		Assert.Equal(bindingContext, lazyView.Content.BindingContext);
	}
}