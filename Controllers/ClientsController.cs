using HomeBanking.Models;
using HomeBanking.Models.dtos;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        //private IAccountRepository _accountRepository;
        //private ICardRepository _cardRepository;
        private AccountsController _accountsController;
        private CardsController _cardsController;
        public ClientsController(IClientRepository clientRepository, AccountsController accountsController, CardsController cardsController)
        {
            _clientRepository = clientRepository;
            _accountsController = accountsController;
            _cardsController = cardsController;
            //_accountRepository = accountRepository;
            //_cardRepository = cardRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = new List<ClientDTO>();
                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        //Cuentas bancarias:
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number
                        }).ToList(),
                        //Prestamos:
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        //Tarjetas:
                        Cards = client.Cards.Select(cr => new CardDTO
                        {
                            Id = cr.Id,
                            CardHolder = cr.CardHolder,
                            Type = cr.Type,
                            Color = cr.Color,
                            Number = cr.Number,
                            Cvv = cr.Cvv,
                            FromDate = cr.FromDate,
                            ThruDate = cr.ThruDate,
                        }).ToList(),
                    };
                    clientsDTO.Add(newClientDTO);
                }

                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return NotFound();
                }
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    //Cuentas:
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    //Prestamos:
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    //Tarjetas:
                    Cards = client.Cards.Select(cr => new CardDTO
                    {
                        Id = cr.Id,
                        CardHolder = cr.CardHolder,
                        Type = cr.Type,
                        Color = cr.Color,
                        Number = cr.Number,
                        Cvv = cr.Cvv,
                        FromDate = cr.FromDate,
                        ThruDate = cr.ThruDate,
                    }).ToList(),
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                //Se obtiene el cliente con sesion iniciada
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                //Verificar si existe el usuario
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Validar nombre y apellido
        public bool ValidateName(string nombre)
        {
            //Solo puede contener letras y por lo menos 3 caracteres
            string patron = @"^[a-zA-Z]{3,}$";
            return Regex.IsMatch(nombre, patron);
        }

        //Validar Email 
        public bool ValidateEmail(string email)
        {
            string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, patron);
        }

        //Registrar Cliente y crearle Account automaticamente
        [HttpPost]
        public IActionResult PostClient([FromBody] Client client)
        {
            try
            {
                //Validacion de datos
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                //Verificar si ya existe el usuario
                Client user = _clientRepository.FindByEmail(client.Email);
                if (user != null)
                    return StatusCode(403, "Email está en uso");

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //Validar Nombre
                bool name = ValidateName(client.FirstName);
                //Validar Apellido
                bool lastname = ValidateName(client.LastName);
                if (name != true || lastname != true) 
                    return StatusCode(403, "No utilizar numeros ni caracteres especiales");
                //Validar Email
                bool email = ValidateEmail(client.Email);
                if (email != true)
                    return StatusCode(403, "Direccion de Email invalida");

                //Validar Password:
                if(client.Password.Length < 8)
                    return StatusCode(403, "La contraseña debe contener al menos 8 caracteres");
                
                bool mayus = false;
                bool minus = false;
                bool num = false;
                foreach (Char c in client.Password)
                {
                    if (char.IsUpper(c))
                    {
                        mayus = true;
                    }
                    if (char.IsLower(c))
                    {
                        minus = true;
                    }
                    if (char.IsDigit(c))
                    {
                        num = true;
                    }
                }
                if (mayus == false)
                    return StatusCode(403, "La contraseña debe contener al menos una mayuscula");
                if (minus == false)
                    return StatusCode(403, "La contraseña debe contener al menos una minuscula");
                if (num == false)
                    return StatusCode(403, "La contraseña debe contener al menos un numero");
                
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);

                _accountsController.Post(newClient.Id);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Crear account:
        [HttpPost("current/accounts")]
        public IActionResult PostAccounts()
        {
            try
            {
                //Se obtiene el cliente con sesion iniciada
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == null)
                    return Forbid("Email vacio");

                //Verificar si existe el usuario
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid("El cliente no existe");

                if (client.Accounts.Count > 2)
                    return StatusCode(403, "El cliente ya posee el limite de cuentras registradas.");

                //Recibo la cuenta creada por el AccountController;
                AccountDTO newAccDTO = _accountsController.Post(client.Id);
                if (newAccDTO == null)
                    return StatusCode(500, "Error");

                return Created("", newAccDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //Crea una Card al cliente activo
        [HttpPost("current/cards")]
        public IActionResult PostCards([FromBody] Card card)
        {
            //Se obtiene el cliente con sesion iniciada
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == null)
                return Forbid();

            //Verificar si existe el usuario
            Client client = _clientRepository.FindByEmail(email);
            if (client == null)
                return Forbid();

            //Verificar si el cliente ya tiene el limite de Cards segun el tipo
            int cantCards = 0;
            string cardType = card.Type;

            foreach (Card cd in client.Cards)
            {
                if (cd.Type == cardType)
                    cantCards++;
            }

            if (cantCards > 2)
                return StatusCode(403, "Ya posee 3 tarjetas del mismo tipo.");

            if (String.IsNullOrEmpty(card.Type) || String.IsNullOrEmpty(card.Color) || 
                !Enum.IsDefined(typeof(CardType), card.Type) || !Enum.IsDefined(typeof(CardColor), card.Color))
                return StatusCode(403, "Seleccione Tipo y Color");

            CardDTO newCardDTO = _cardsController.Post($"{client.FirstName} {client.LastName}", card.Type, card.Color, client.Id);

            if (newCardDTO == null)
                return StatusCode(500, "Error");

            return Created("", newCardDTO);
        }

        //JSON con las Cards del cliente activo
        [HttpGet("current/cards")]
        public IActionResult GetClientCards()
        {
            try
            {
                //Se obtiene el cliente con sesion iniciada
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                //Verificar si existe el usuario
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                var newCardsDTO = client.Cards.Select(cd => new CardDTO 
                {
                    Id = cd.Id,
                    CardHolder = cd.CardHolder,
                    Type = cd.Type,
                    Color = cd.Color,
                    Number = cd.Number,
                    Cvv = cd.Cvv,
                    FromDate = cd.FromDate,
                    ThruDate = cd.ThruDate,
                }).ToList();

                return Ok(newCardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        //JSON con las Accounts del cliente activo
        [HttpGet("current/accounts")]
        public IActionResult GetClientAccounts()
        {
            try
            {
                //Se obtiene el cliente con sesion iniciada
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                //Verificar si existe el usuario
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                var newAccountsDTO = client.Accounts.Select(ac => new AccountDTO 
                { 
                    Id = ac.Id,
                    Number = ac.Number,
                    CreationDate = ac.CreationDate,
                    Balance = ac.Balance,
                    //Transactions = ac.Transactions.Select(ts => new TransactionDTO 
                    //{
                    //    Id = ts.Id,
                    //    Type = ts.Type,
                    //    Amount = ts.Amount,
                    //    Description = ts.Description,
                    //    Date = ts.Date,
                    //}).ToList(),
                }).ToList();

                return Ok(newAccountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
