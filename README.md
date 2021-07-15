# Image-Drive-with-base64

Resim Depolama Sitesi(http://52.59.124.115:81/)

C# Asp.net Framework’ün MVC mimarisi ile hazırlanan bu sitede MSSQL veritabanı kullanılmıştır.

Kullanıcı kaydından sonra resimlerin upload edilebileceği arayüze erişim sağlanıyor. Yüklenen resimler base64 Encoding 
yöntemi ile kodlanıyor. Böylelikle kullanıcı giriş yapmışsa sadece yüklediği resimleri görüyor, kendisinin yüklememiş 
olduğu resimlerin Server’daki yolunu bilse dahi erişim sağlaması engelleniyor, yüklenen resimler güvenli hale geliyor.
