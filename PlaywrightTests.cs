using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace PlayWright.Tests;
/// <summary>
/// when running from main solution and ServiceTestSetup.[AssemblyInitialize] is invoked, it's by some reason headless.
/// If you need to debug output, run standalone PlayWright.Tests and disable [AssemblyInitialize] in ServiceTestSetup
/// </summary>
[TestClass]
public class PlaywrightTests : PageTest
{

    private string _baseUrl = "http://localhost:5000/";// TestContext.Parameters["BaseUrl"]

    [TestInitialize] //should be after base PageTest.PageSetup https://stackoverflow.com/questions/18511269/multiple-testinitialize-attributes-in-mstest
    public async Task OpenPage()
    {
        // Set the static TestContext property,allows  to indirectly log initialization messages through each test’s TestContext.
        ServiceTestSetup.TestContext = TestContext;
        if (ServiceTestSetup.ServiceProcess?.HasExited ?? true)
        { //try to start for the test instead of AssemblyInitialize
            ServiceTestSetup.Initialize(TestContext);
            if (ServiceTestSetup.ServiceProcess?.HasExited??true)
            {
                Assert.Fail("ServiceProcess has exited. See logs for the reason");
            }
        }
#if !DEBUG
        _baseUrl = "http://localhost:8080/"; //in docker app, it's started as 8080, TODO move to config or check isOSLinux
#endif
        //TestContext.WriteLine(System.Text.Json.JsonSerializer.Serialize(TestContext.Properties)); // TestContext.Parameters["BaseUrl"]
        await Page.GotoAsync(_baseUrl);
    }
    [TestMethod]
    public async Task SearchCellInRowSingleLocator()
    {
        await Page.GotoAsync("https://www.w3schools.com/html/html_tables.asp");
        int rowNumber = 3;
        int cellNumber = 3;
        var cellLocator = Page.Locator($"table.ws-table-all tbody tr:nth-child({rowNumber + 1}) td:nth-child({cellNumber})");
        var text = await cellLocator.TextContentAsync();
        Assert.IsTrue(text == "Austria");
    }
}