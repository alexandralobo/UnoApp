using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class CardController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<ICollection<Card>> GetDeck()
        {
            return Ok(_unitOfWork.CardRepository.GetDeck());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(int id)
        {
            return await _unitOfWork.CardRepository.GetCard(id);
        }


    }
}