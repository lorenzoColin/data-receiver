﻿using data_receiver.Models.ViewModels;
using data_receiver.Models;
using Nest;
using data_receiver.ElasticConnection;
using Microsoft.AspNetCore.Mvc;
using data_receiver.Data;

public class CustomerList
{
    private readonly ElasticClient _client;
    private readonly ApplicationDbContext _db;


    public CustomerList([FromServices] ApplicationDbContext db)
    {
        _client = new ElasticSearchClient().EsClient();
        _db = db;
    }

    //list of the logged in user and associated customers
    public List<CustomerViewModel> unclaimedCustomerlist(string loggedinuser)
    {
        //sma klanten
        var search = _client.Search<Sma_klanten>(s => s.Index("sma_klanten"));
        var sma_klanten = search.Documents;
        var CustomerViewModel = new List<CustomerViewModel>();

        //dit zijn alle sma klanten
        foreach (var sma in sma_klanten)
        {
            CustomerViewModel.Add(new CustomerViewModel
            {
                customer = new Customer
                {
                    Debiteurnr = sma.Debiteurnr,
                    Klant = sma.Klant,
                    Consultant = sma.Beheerder,
                    Datum_live = sma.datum_live,
                    Max_budget = sma.budget_afspr,
                    Doelstelling = sma.Doelstelling,
                    Servicefee_afspraak = sma.Servicefee_afspraak,
                    CustomerType = "sma_klanten"
                }
            });
        }

        var search1 = _client.Search<Customer>(s => s.Index("sea_klanten"));
        var sea_klanten = search1.Documents;

        foreach (var sea in sea_klanten)
        {
            CustomerViewModel.Add(new CustomerViewModel
            {
                customer = sea,
            });
            sea.CustomerType = "sea_klanten";
        }

        //dit zijn alle klanten van sma en sea
        var AllCustomers = CustomerViewModel;

        //my customers
        IEnumerable<UserCustomer> userCustomers = _db.UserCustomer;
        //list to add my customers
        var mycustomers = new List<CustomerViewModel>();
        foreach (var mycustomer in userCustomers)
        {
            //mijn user ophalen vanuit die table
            if (mycustomer.userid == loggedinuser)
            {
                foreach (var test in AllCustomers)
                {
                    if (mycustomer.DebiteurnrId == test.customer.Debiteurnr && mycustomer.customerType == test.customer.CustomerType)
                    {
                        mycustomers.Add(new CustomerViewModel { customer = test.customer, customerType = test.customerType });
                    }
                }
            }
        }
        var customersToRemove = new List<CustomerViewModel>();
        foreach (var customer in AllCustomers)
        {
            foreach (var myCustomer in mycustomers)
            {
                if (myCustomer.customer.CustomerType == customer.customer.CustomerType && myCustomer.customer.Debiteurnr == customer.customer.Debiteurnr)
                {
                    customersToRemove.Add(customer);
                }
            }
        }
        var result = AllCustomers.Except(customersToRemove).ToList();

        return result;
    }


    public List<CustomerViewModel> claimedcustomerlist(string loggedInUser)
    {

        //sma klanten
        var search = _client.Search<Sma_klanten>(s => s.Index("sma_klanten"));
        var sma_klanten = search.Documents;
        var CustomerViewModel = new List<CustomerViewModel>();


        //dit zijn alle sma klanten
        foreach (var sma in sma_klanten)
        {
            CustomerViewModel.Add(new CustomerViewModel
            {
                customer = new Customer
                {
                    Debiteurnr = sma.Debiteurnr,
                    Klant = sma.Klant,
                    Consultant = sma.Beheerder,
                    Datum_live = sma.datum_live,
                    Max_budget = sma.budget_afspr,
                    Doelstelling = sma.Doelstelling,
                    Servicefee_afspraak = sma.Servicefee_afspraak,
                    CustomerType = "sma_klanten"
                }
            });
        }

        var search1 = _client.Search<Customer>(s => s.Index("sea_klanten"));
        var sea_klanten = search1.Documents;
        foreach (var sea in sea_klanten)
        {
            CustomerViewModel.Add(new CustomerViewModel
            {
                customer = sea,
            });
            sea.CustomerType = "sea_klanten";
        }

        //dit zijn alle klanten van sma en sea
        var AllCustomers = CustomerViewModel;

        //my customers
        IEnumerable<UserCustomer> userCustomers = _db.UserCustomer;
        //list to add my customers
        var mycustomers = new List<CustomerViewModel>();
        foreach (var mycustomer in userCustomers)
        {
            //mijn user ophalen vanuit die table
            if (mycustomer.userid == loggedInUser)
            {
                foreach (var test in AllCustomers)
                {
                    if (mycustomer.DebiteurnrId == test.customer.Debiteurnr && mycustomer.customerType == test.customer.CustomerType)
                    {
                        mycustomers.Add(new CustomerViewModel { customer = test.customer, customerType = test.customerType });
                    }
                }

                //var searchclient = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(mycustomer.DebiteurnrId))));
                //var singlecustomer = searchclient.Documents.FirstOrDefault(s => s.Debiteurnr == mycustomer.DebiteurnrId);
                //mycustomers.Add(singlecustomer.Debiteurnr, singlecustomer.Contact);
            }
        }

        return mycustomers;
    }



    //list with all customers
    public List<CustomerViewModel> getallcustomers()
    {

        //sma klanten
        var search = _client.Search<Sma_klanten>(s => s.Index("sma_klanten"));
        var sma_klanten = search.Documents;
        var CustomerViewModel = new List<CustomerViewModel>();


        //dit zijn alle sma klanten
        foreach (var sma in sma_klanten)
        {
            CustomerViewModel.Add(new CustomerViewModel
            {
                customer = new Customer
                {
                    Debiteurnr = sma.Debiteurnr,
                    Klant = sma.Klant,
                    Consultant = sma.Beheerder,
                    Datum_live = sma.datum_live,
                    Max_budget = sma.budget_afspr,
                    Doelstelling = sma.Doelstelling,
                    Servicefee_afspraak = sma.Servicefee_afspraak,
                    CustomerType = "sma_klanten"
                }
            });
        }

        var search1 = _client.Search<Customer>(s => s.Index("sea_klanten"));
        var sea_klanten = search1.Documents;
        foreach (var sea in sea_klanten)
        {
            CustomerViewModel.Add(new CustomerViewModel
            {
                customer = sea,
            });
            sea.CustomerType = "sea_klanten";
        }

        

        return CustomerViewModel;

    }
}