    using System;

    /*
    namespace PBL
    {
        internal class Program
        {

            static private Random rastgele = new Random();

            static void Main(string[] args)
            {
                Console.Clear(); //çizimlere net geçilmesi için ekranı temizledik
                int UIWidth = 30;
                int UIHeight = 20;
                UI(UIWidth, UIHeight);
      
                Console.Write("Enter piece count for how many matrix you want: ");
                string[] harfSayıMetniDizisi = Console.ReadLine().Split(' '); // boşluklarda .Split('') ile ayırıp dizeye eleman kaydettik
                int[] harfSayıTamsayıDizisi = new int[harfSayıMetniDizisi.Length]; // parça sayısı için int dizisi oluşturduk
                char[] harfListesi = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' };

                if (harfSayıMetniDizisi.Length > 20)
                {
                    Console.SetCursorPosition(0, UIHeight + 3);
                    Console.WriteLine("You can enter maximum 20 pieces.");
                    return; // 20den fazla parça istenirse main fonksiyonu pas geç debug ı
                }
                else
                {
                    for (int i = 0; i < harfSayıMetniDizisi.Length; i++)
                        harfSayıTamsayıDizisi[i] = Convert.ToInt32((harfSayıMetniDizisi[i]));

                    // parçaların ekrana yazıldıgı kısım
                    int konumSatir = 1;
                    int parçaSayacı = 0;

                    //konumSatir += 7: 5x5 matrixde çalışıyoruz. basımda 2 satır boşluk kalması için 5+2
                    for (int i = 0; i < 4; i++, konumSatir += 7)
                    {
                        if (parçaSayacı >= harfSayıMetniDizisi.Length)
                        {
                            break;// son parça tamamlandı. işlem bitti. ?????
                        }

                        // Parçaları arayüzün SAĞINA hizaladık
                        int konumSutun = UIWidth + 5;

                        // çizdirmeler satır satır yapılmakta. nested loop ile yana kayarak çizim devam etmesini sağladık
                        for (int j = 0; j < 5; j++, konumSutun += 10)
                        {
                            //burada çizen fonksiyonu çagırıp çizmeyi sağladık
                            Parça_Oluştur(harfSayıTamsayıDizisi[parçaSayacı], konumSutun, konumSatir, harfListesi[parçaSayacı]);
                            parçaSayacı++;

                            if (parçaSayacı >= harfSayıMetniDizisi.Length)
                            {
                                break; // her parça çizildiğinde kodu durdurdu ??????? (neden çift kontrol ögren)
                            }
                        }
                    }
                    Console.SetCursorPosition(0, UIHeight + 3);
                    Console.Write("Press any key to exit...");
                    Console.ReadKey();
                }
            } // Main fonksiyonu burada bitti

            // Parça oluşum fonksiyonu (harfSayısı, konumSutun, konumSatir). üç kriterde
            static void Parça_Oluştur(int harfSayısı, int konumSutun, int konumSatir, char parçaHarfi)
            {
                int[,] parçalar = new int[5, 5];
                int[] harfSatırları = new int[harfSayısı];
                int[] harfSütunları = new int[harfSayısı];

                int sayaç = 0; // kaç kare yerleştirdiğimizi tutar
                bool harfMi = false; // harf mi * mı?

                int ilkHarfSatırı = rastgele.Next(0, 5);
                int ilkHarfSütunu = rastgele.Next(0, 5);

                // yayılma çizimi için ilk noktayı kayıt etdik
                harfSatırları[sayaç] = ilkHarfSatırı;
                harfSütunları[sayaç] = ilkHarfSütunu;
                parçalar[ilkHarfSatırı, ilkHarfSütunu] = 1; //dolu parça == 1
                sayaç++; // Sayaç arttı. ilk kare yerleşmiş oldu

                //0. array yerleşti. yayılım ile istenen adete ulaşana kadar devam ettiren döngü
                for (int i = 1; i < harfSayısı; i++)
                {
                    // Amaç: O ana kadar yerleştirilmiş karelerden BİRİNİ rastgele seçmek
                    int rastgeleHarfSeç = rastgele.Next(0, sayaç);
                    int yön = rastgele.Next(1, 5);// random yönde yayılım

                    //taşma engelledik vb durum engelledi (yön debug kısmı)
                    if (yön == 1 && harfSatırları[rastgeleHarfSeç] != 0 && parçalar[harfSatırları[rastgeleHarfSeç] - 1, harfSütunları[rastgeleHarfSeç]] != 1) // !=1 doluluk kontrolü
                    {
                        harfSatırları[sayaç] = harfSatırları[rastgeleHarfSeç] - 1; 
                        harfSütunları[sayaç] = harfSütunları[rastgeleHarfSeç]; 
                        parçalar[harfSatırları[sayaç], harfSütunları[sayaç]] = 1; // matrixi dolu == 1 yaptık
                        sayaç++; // Toplam kare sayacını artırdık
                    }
                    // yön sağ ise ( 2)
                    else if (yön == 2 && harfSütunları[rastgeleHarfSeç] != 4 && parçalar[harfSatırları[rastgeleHarfSeç], harfSütunları[rastgeleHarfSeç] + 1] != 1)
                    {
                        harfSütunları[sayaç] = harfSütunları[rastgeleHarfSeç] + 1;
                        harfSatırları[sayaç] = harfSatırları[rastgeleHarfSeç];
                        parçalar[harfSatırları[sayaç], harfSütunları[sayaç]] = 1;
                        sayaç++;
                    }
                    // yön aşagı ise ( 3)
                    else if (yön == 3 && harfSatırları[rastgeleHarfSeç] != 4 && parçalar[harfSatırları[rastgeleHarfSeç] + 1, harfSütunları[rastgeleHarfSeç]] != 1)
                    {
                        harfSatırları[sayaç] = harfSatırları[rastgeleHarfSeç] + 1;
                        harfSütunları[sayaç] = harfSütunları[rastgeleHarfSeç];
                        parçalar[harfSatırları[sayaç], harfSütunları[sayaç]] = 1;
                        sayaç++;
                    }
                    // yön sol ise ( 4)
                    else if (yön == 4 && harfSütunları[rastgeleHarfSeç] != 0 && parçalar[harfSatırları[rastgeleHarfSeç], harfSütunları[rastgeleHarfSeç] - 1] != 1)
                    {
                        harfSütunları[sayaç] = harfSütunları[rastgeleHarfSeç] - 1;
                        harfSütunları[sayaç] = harfSatırları[rastgeleHarfSeç];
                        parçalar[harfSatırları[sayaç], harfSütunları[sayaç]] = 1;
                        sayaç++;
                    }
                    // HİÇBİRİ OLMADIYSA (ya sınır dışına taştı ya da dolu bir yere denk geldi)
                    else
                        i--;// Bu deneme başarısız oldu. döngü aynı sayıdan devam etsin.

                }

                // parça oluşmuş oldu. bu blokta çizdirmek için 0-1 leri yazdırıyor
                for (int satir = 0; satir < 5; satir++, konumSatir++)
                {
                    Console.SetCursorPosition(konumSutun, konumSatir);

                    for (int sutun = 0; sutun < 5; sutun++)
                    {

                        for (int i = 0; i < harfSayısı; i++)
                        {
                            if (satir == harfSatırları[i] && sutun == harfSütunları[i])
                            {
                                Console.Write(parçaHarfi);
                                harfMi = true;
                                break;
                            }
                            else
                            {
                                harfMi = false; // bu koordinat için henüz atama yok
                            }
                        }

                        if (harfMi == false)
                        {
                            Console.Write("*");
                        }
                    }
                }
            }

            static void UI(int genişlik, int yükseklik)
            {
                Console.SetCursorPosition(1, 1);
                Console.Write("+");
                for (int i = 0; i < genişlik; i++)
                    Console.Write("-");
                Console.Write("+");

                for (int i = 0; i < yükseklik; i++)
                {
                    Console.SetCursorPosition(1, 2 + i);
                    Console.Write("|");
                    Console.SetCursorPosition(2 + genişlik, 2 + i);
                    Console.Write("|");
                }

                Console.SetCursorPosition(1, 2 + yükseklik);
                Console.Write("+");
                for (int i = 0; i < genişlik; i++) Console.Write("-");
                Console.Write("+");

                Console.SetCursorPosition(0, 0);

            }
        }
    } */

































