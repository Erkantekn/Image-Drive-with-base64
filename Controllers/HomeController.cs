using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinalDrive_v2._0.Models;
namespace FinalDrive_v2._0.Controllers
{
    public class HomeController : Controller
    {
        dataEntities db = new dataEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(users kullanici)
        {
            var kullaniciDb = db.users.FirstOrDefault(x => x.kAdi == kullanici.kAdi && x.sifre == kullanici.sifre);
            if (kullaniciDb == null) 
            {
                ViewBag.hata = "Giriş Bilgileri Hatalı!";
                return View();
                
            }
            else 
            { 
                FormsAuthentication.SetAuthCookie(kullaniciDb.kAdi, false);
                return RedirectToAction("Gallery");
            }
{
}

        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            ViewBag.hata = "Başarıyla Çıkış Yapıldı.";
            return View("Login");
        }

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(users kullanici,string re_sifre)
        {
            if(db.users.FirstOrDefault(x => x.kAdi == kullanici.kAdi)!=null)
            {
                ViewBag.hata = "Kullanıcı Adı Daha Önce Kullanılmış.";
                
            }
            else if(kullanici.sifre != re_sifre)
            {
                ViewBag.hata = "Şifreler Birbirinden Farklı.";
                
            }
            else
            {
                db.users.Add(kullanici);
                db.SaveChanges();
                ViewBag.hata = "Kullanıcı Kaydınız Başarıyla Oluşturuldu.";
            }
            return View();
        }
        [Authorize]
        public ActionResult Gallery(string hata)
        {

            if (hata != null)
                ViewBag.hata = hata;

            List<photos> phts = db.photos.Where(x => x.users.kAdi == User.Identity.Name).ToList();
            users usr = db.users.FirstOrDefault(x => x.kAdi == User.Identity.Name);
            if (usr != null)
            {
                ViewBag.storage = Convert.ToInt32( usr.kullanılanAlan)*100/250;
            }
            
            return View(phts);
        }
        [HttpPost]
        [Authorize]
        public ActionResult GetPhotoFull(int? id)
        {


            //gelen id yi Veritabanımızda kontrol ediyoruz ve gelen employee partialviewimize gönderiyoruz
            var model = db.photos.FirstOrDefault(x => x.id == id);
            if (model != null)
            {

                return PartialView(model);
            }
            else
                return HttpNotFound();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Upload(HttpPostedFileBase photo)
        {
            if (photo != null && photo.ContentLength > 0)
            {
                string ServerMapPath = Server.MapPath("~\\images\\users\\" + User.Identity.Name);
                //Dosya Açılmış mı kontrol edelim
                if (!Directory.Exists(ServerMapPath))
                {
                    //Eğer dosya Yoksa yeni klasör oluşturuyoruz.
                    Directory.CreateDirectory(ServerMapPath);
                }
                
               
                photos pht = new photos();
                pht.boyut = photo.ContentLength / 1048576.0;
                users usr = db.users.FirstOrDefault(x => x.kAdi == User.Identity.Name);
                if (usr != null)
                {
                    //250 MB'den büyük mü
                    if(usr.kullanılanAlan + pht.boyut > 250.0)
                    {
                        ViewBag.hata = "250 MB Kullanım Sınırını Aştınız.";
                        return RedirectToAction("Gallery", new{hata = ViewBag.hata});
                    }
                }
                else
                    HttpNotFound();
                string pathForDb = Guid.NewGuid() + photo.FileName;
                string path = ServerMapPath + "\\" +pathForDb;
                photo.SaveAs(path);

                pht.userId = usr.id;
                pht.yol = pathForDb;
                usr.kullanılanAlan += pht.boyut;
                db.photos.Add(pht);
                db.SaveChanges();

                ViewBag.hata = "Fotoğraf Başarıyla Yüklendi";
                return RedirectToAction("Gallery", new{hata = ViewBag.hata});
            }
            else
            {
                ViewBag.hata = "Fotoğraf Yüklenirken Bir Hata Oluştu.";
                return RedirectToAction("Gallery", new{hata = ViewBag.hata});
            }
        }


        [Authorize]
        public ActionResult Delete(int?[] id)
        {
                if (id != null)
                {

            
                    foreach (int item in id)
                    {
                        var pht = db.photos.FirstOrDefault(x => x.id == item);
                        if (pht != null)
                        {
                            users usr = db.users.FirstOrDefault(x => x.kAdi == User.Identity.Name);
                            if(pht.userId != usr.id)
                            {
                                ViewBag.hata = "Silinmek istenen resim için yetkiniz yok.";
                                return RedirectToAction("Gallery", new { hata = ViewBag.hata });
                            }
                    
                            if(System.IO.File.Exists(Server.MapPath("~\\images\\users\\" + User.Identity.Name+"\\"+pht.yol)))
                            {
                                System.IO.File.Delete(Server.MapPath("~\\images\\users\\" + User.Identity.Name + "\\" + pht.yol));
                                usr.kullanılanAlan -= pht.boyut;
                                db.photos.Remove(pht);
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewBag.hata = "Silinmek istenen resim bulunamadı.";
                                return RedirectToAction("Gallery", new { hata = ViewBag.hata });
                            }
                    

                        }
                        else
                        {
                            ViewBag.hata = "Resim Silme İşlemi Başarısız. Silinmek istenen resim bulunamadı.";
                            return RedirectToAction("Gallery", new { hata = ViewBag.hata });
                        }
                    }
                    ViewBag.hata = "Silme işlemi Başarılı. "+id.Length.ToString()+" Adet Resim Silindi.";
                    return RedirectToAction("Gallery", new { hata = ViewBag.hata });
                }
                else
                {
                ViewBag.hata = "Boş değer girişi yapıldı.";
                return RedirectToAction("Gallery", new { hata = ViewBag.hata });
            }
        }





        static string base64String = null;
        public string ImageToBase64(string path)
        {
             path = Path.Combine(HttpRuntime.AppDomainAppPath, path);
            
            //string path = "D:\\SampleImage.jpg";
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }
    
     
    }
}