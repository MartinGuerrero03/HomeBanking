using HomeBanking.Models;
using HomeBanking.Models.dtos;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private ICardRepository _cardRepository;

        public CardsController(ICardRepository cardRepository) 
        {
            _cardRepository = cardRepository;
        }

        [HttpPost]
        public CardDTO Post(string cardHolder, string type, string color, long clientId) 
        {
            Random random = new Random();
            string cardNumber;
            Card cardAux;
            try 
            {
                //Se crean los datos para la nueva tarjeta y se valida que no se repita el numero de tarjeta
                do
                {
                    cardNumber = $"{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
                    cardAux = _cardRepository.FindByNumber(cardNumber);
                }
                while (cardAux != null);

                Card newCard = new Card
                {
                    CardHolder = cardHolder,
                    Type = type,
                    Color = color,
                    Number = cardNumber,
                    Cvv = random.Next(100, 999),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(4),
                    ClientId = clientId
                };

                _cardRepository.Save(newCard);

                CardDTO newCardDTO = new CardDTO 
                {
                    CardHolder = newCard.CardHolder,
                    Type = newCard.Type,
                    Color = newCard.Color,
                    Number = newCard.Number,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    ThruDate = newCard.ThruDate,
                    Id = newCard.Id
                };

                return newCardDTO;
            }
            catch
            {
                return null;
            }
        }

    }
}
