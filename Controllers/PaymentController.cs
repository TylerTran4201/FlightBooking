using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;

namespace FlightBooking.Controllers
{
    public class PaymentController : Controller
    {
        public DataContext _context { get; }
        public UserManager<AppUser> _userManager { get; }
        public IHttpContextAccessor _httpContext { get; }
        public IConfiguration _configuration { get; }
        public PaymentController(DataContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContext, IConfiguration iconfiguration)
        {
            _configuration = iconfiguration;
            _httpContext = httpContext;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(int scheduleId, string listSeatSelected)
        {
            var schedule = await _context.Schedules.Include(s => s.Airline).ThenInclude(a => a.Seats)
                .Include(a => a.Airline.AirlineCompany)
                .Include(s => s.DepartureAirport)
                .Include(s => s.DestinationAirport)
                .Where(s => s.Id == scheduleId).FirstOrDefaultAsync();

            var totalPrice = 0.0;
            var bussinessPrice = Math.Round((double)_context.TypeSeats.Where(p => p.Id == 1).Select(p => p.Price).FirstOrDefault(), 1);
            var selectedList = listSeatSelected.Split(',');
            //take total price
            List<double> prices = new List<double>();
            List<Seat> seats = new List<Seat>();
            foreach (var item in selectedList)
            {
                var price = schedule.Airline.BasePrice;
                var seat = schedule.Airline.Seats.Where(a => a.Name.Equals(item)).FirstOrDefault();
                seats.Add(seat);

                if (seat.TypeSeatId == 1)
                {
                    totalPrice += price + bussinessPrice;
                    prices.Add(price + bussinessPrice);
                }
                else
                {
                    totalPrice += price;
                    prices.Add(price);
                }
            }

            ViewBag.NumOfTicket = selectedList.Length;
            ViewBag.TotalPrice = totalPrice;
            ViewBag.Schedule = schedule;
            ViewBag.Prices = prices.ToArray();
            ViewBag.Trip = (schedule.DepartureAirport.city + " - " + schedule.DestinationAirport.city).ToString();

            TempData["ScheduleId"] = schedule.Id;
            TempData["ListSeat"] = listSeatSelected;
            TempData["TotalPrice"] = totalPrice.ToString();

            return View();
        }
        public async Task<int> AddBooking(int scheduleId, Seat[] seats)
        {
            var user = await _context.Users.Where(u => u.UserName.CompareTo(_userManager.GetUserName(User)) == 0).FirstOrDefaultAsync();
            var bookingId = int.Parse(user.Id.ToString() + DateTime.Now.ToString("HHmmss"));
            var booking = new Booking
            {
                Id = bookingId,
                UserId = user.Id,
                ScheduleId = scheduleId
            };
            _context.Add(booking);
            await _context.SaveChangesAsync();

            int count = 0;

            var passengerInfos = GlobalVariables.PassengerInformationDtos.ToArray();
            foreach (var item in seats)
            {
                var passengerInfoId = int.Parse(user.Id.ToString() + DateTime.Now.ToString("HHmmss") + count.ToString());
                var passengerInfo = new PassengerInformation
                {
                    Id = passengerInfoId,
                    FullName = passengerInfos[count].FullName,
                    IdCard = passengerInfos[count].IdCard
                };
                var ticket = new Ticket
                {
                    SeatId = seats[count].Id,
                    BookingId = bookingId,
                    PassengerInformationId = passengerInfo.Id
                };
                count++;
                _context.Add(passengerInfo);
                _context.Add(ticket);
                await _context.SaveChangesAsync();
            }
            return bookingId;
        }

        public async void AddBill(int bookingId, double totalPrice)
        {
            var order = new Bill
            {
                BookingId = bookingId,
                TotalPrice = totalPrice
            };
            _context.Add(order);
            await _context.SaveChangesAsync();
        }

        public async void OffSeat(List<Seat> seats)
        {
            foreach (var item in seats)
            {
                item.Status = 1;
                _context.Update(item);
            }
            await _context.SaveChangesAsync();
        }
        //paypal
        private PayPal.Api.Payment payment;

        public async Task<bool> CreateBill(int scheduleId, string seatsSelectedStr, double totalPrice){
            try {
                if (scheduleId != 0 && seatsSelectedStr != null && totalPrice != 0)
                {
                    var nameSeats = seatsSelectedStr.Split(',');
                    var schedule = await _context.Schedules.Include(s => s.Airline).ThenInclude(a => a.Seats).Where(s => s.Id == scheduleId).FirstOrDefaultAsync();
                    var seats = new List<Seat>();

                    foreach (var item in nameSeats)
                    {
                        var seat = schedule.Airline.Seats.Where(a => a.Name.Equals(item)).FirstOrDefault();
                        seats.Add(seat);
                    }

                    var bookingId = await AddBooking(scheduleId, seats.ToArray());
                    OffSeat(seats);
                    AddBill(bookingId, totalPrice);
                }
                return true;
            } catch (Exception e) {
                return false;
            }
        }
        public async Task<ActionResult> PaymentWithPaypal(string Cancel = null, string PayerId = "",string guid = "")
        {
            var ClientID = _configuration.GetValue<string>("PayPal:Key");
            var ClientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
            try{
                string payerId = PayerId;
                if (string.IsNullOrEmpty(payerId)){
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Payment/PaymentWithPayPal?";
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext()){
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }                                                
                    }
                    _httpContext.HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }else{
                    var paymentId = _httpContext.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PaymentFailed");
                    }

                    // Create Bill
                    var scheduleId = int.Parse(TempData["ScheduleId"].ToString());
                    var seatsSelectedStr = TempData["ListSeat"].ToString();
                    var totalPrice = double.Parse(TempData["TotalPrice"].ToString());
                    await CreateBill(scheduleId, seatsSelectedStr, totalPrice);
                    return View("PaymentSuccess");
                }
            }catch (Exception ex)
            {
                return View("PaymentFailed");
            }
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            var totalPrice = double.Parse(TempData["TotalPrice"].ToString());
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Airline Ticket",
                currency = "USD",
                price = totalPrice.ToString(),
                quantity = "1",
                sku = "asd"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = totalPrice.ToString(),
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
    }
}