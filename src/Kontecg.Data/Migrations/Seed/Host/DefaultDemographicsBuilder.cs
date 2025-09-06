using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Kontecg.EFCore;
using Kontecg.HumanResources;
using Kontecg.Json;
using Kontecg.Timing;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed.Host
{
    public class DefaultDemographicsBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly IContentFolders _contentFolders;
        private readonly ILogger _logger;

        private static List<Country> InitialCountries => GetInitialCountries();

        private static List<State> InitialStates => GetInitialStates();

        public DefaultDemographicsBuilder(KontecgCoreDbContext context, IContentFolders contentFolders, ILogger logger)
        {
            _context = context;
            _contentFolders = contentFolders;
            _logger = logger;
        }

        public void Create()
        {
            CreateCountries();
            CreateStates();
            CreateCities();

            CreateSpecialDates(2025);
            CreateSpecialDates(2026);

            AddSpecialDateIfNotExists(new DateTime(2025, 1, 3));
            AddSpecialDateIfNotExists(new DateTime(2025, 1, 4));
            AddSpecialDateIfNotExists(new DateTime(2025, 4, 18));
            AddSpecialDateIfNotExists(new DateTime(2025, 5, 2));

            _logger.Info("Special dates added.");

            ImportPersonsFromExternalDatabase(_contentFolders.DataFolder);
        }

        private void CreateSpecialDates(int year)
        {
            AddSpecialDateIfNotExists(new DateTime(year, 1, 1), DayDecorator.NationalCelebrationDay);
            AddSpecialDateIfNotExists(new DateTime(year, 1, 2), DayDecorator.NationalHoliday);
            AddSpecialDateIfNotExists(new DateTime(year, 5, 1), DayDecorator.NationalCelebrationDay);
            AddSpecialDateIfNotExists(new DateTime(year, 7, 25), DayDecorator.NationalHoliday);
            AddSpecialDateIfNotExists(new DateTime(year, 7, 26), DayDecorator.NationalCelebrationDay);
            AddSpecialDateIfNotExists(new DateTime(year, 7, 27), DayDecorator.NationalHoliday);
            AddSpecialDateIfNotExists(new DateTime(year, 10, 10), DayDecorator.NationalCelebrationDay);
            AddSpecialDateIfNotExists(new DateTime(year, 12, 25), DayDecorator.NationalHoliday);
            AddSpecialDateIfNotExists(new DateTime(year, 12, 31), DayDecorator.NationalHoliday);

            var initial = new DateTime(year, 1, 1);
            var even = 1;
            while (initial <= new DateTime(year, 12, 31))
            {
                if (initial.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (even % 2 == 0) 
                        AddSpecialDateIfNotExists(initial, DayDecorator.BreakSaturday);
                    even++;
                }
                initial = initial.AddDays(1);
            }
        }

        private void CreateCountries()
        {
            foreach (var country in InitialCountries)
            {
                AddCountryIfNotExists(country);
            }
            _logger.Info("Countries created.");
        }

        private void CreateStates()
        {
            var cuba = _context.Countries.IgnoreQueryFilters().FirstOrDefault(r => r.Acronym == "CUB");
            if (cuba == null)
            {
                cuba = _context.Countries.Add(new Country("EL CARIBE", "Cuba", "CUB", "192")).Entity;
                _context.SaveChanges();
            }

            foreach (var state in InitialStates)
            {
                state.Country = cuba;
                AddStateIfNotExists(state);
            }
            _logger.Info("States created.");
        }

        private void CreateCities()
        {
            var cities = GetInitialCities();
            foreach (string[] city in cities)
            {
                AddCitiesIfNotExists(city[0], city[1], city[2]);
            }
            _logger.Info("Cities created.");
        }

        private void AddCountryIfNotExists(Country country)
        {
            if (_context.Countries.IgnoreQueryFilters().Any(c => c.Name == country.Name))
                return;

            _context.Countries.Add(country);
            _context.SaveChanges();
        }

        private void AddStateIfNotExists(State state)
        {
            if (_context.States.IgnoreQueryFilters().Any(c => c.Name == state.Name && c.Code == state.Code && c.Country.InternationalCode == state.Country.InternationalCode))
                return;

            _context.States.Add(state);
            _context.SaveChanges();
        }

        private void AddCitiesIfNotExists(string name, string code, string state)
        {
            if (_context.Cities.IgnoreQueryFilters().Any(c => c.Name == name && c.Code == code && c.State.Code == state))
                return;

            var st = _context.States.IgnoreQueryFilters().FirstOrDefault(r => r.Code == state);
            if (st == null) return;

            _context.Cities.Add(new City(name, code) {State = st});
            _context.SaveChanges();
        }

        private void ImportPersonsFromExternalDatabase(string dataFolder)
        {
            try
            {
                var tempDataFile = Path.GetFullPath(Path.Combine(dataFolder, "seed", "persons.json"));

                if (File.Exists(tempDataFile))
                {
                    var content = File.ReadAllText(tempDataFile, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(content))
                    {
                        var persons = content.FromJsonString<Person[]>();
                        var personsToSave =
                            persons.ExceptBy(_context.Persons.IgnoreQueryFilters().Select(p => p.IdentityCard),
                                p => p.IdentityCard).ToArray();


                        foreach (Person person in personsToSave)
                        {
                            _context.Persons.Add(person);
                            _logger.Info($"---> {person.FullName} Added.");
                        }

                        _context.SaveChanges();
                    }
                    _logger.Info("Persons was populated from external data.");
                }
            }
            catch(Exception ex)
            {
                _logger.Warn("Persons couldn't be retrieved from external data.", ex);
            }
        }

        private void AddSpecialDateIfNotExists(DateTime nonWorkingDay, DayDecorator cause = DayDecorator.BreakDay)
        {
            if (_context.SpecialDates.IgnoreQueryFilters().Any(s => s.Date == nonWorkingDay && s.Cause == cause))
                return;

            _context.SpecialDates.Add(new SpecialDate(nonWorkingDay, cause));
            _context.SaveChanges();
        }

        #region Countries

        private static List<Country> GetInitialCountries()
        {
            return new List<Country>
            {
                //new Country("MUNDO", "MUNDO", "---", "001"),
                //new Country("AFRICA", "AFRICA", "---", "002"),
                //new Country("ASIA MERIDIONAL", "Afganistán", "AFG", "004"),
                //new Country("AMERICA SUR", "AMERICA SUR", "---", "005"),
                //new Country("EUROPA MERIDIONAL", "Albania", "ALB", "008"),
                //new Country("OCEANIA", "OCEANIA", "---", "009"),
                //new Country("AFRICA OCCIDENTAL", "AFRICA OCCIDENTAL", "---", "011"),
                //new Country("AFRICA SEPTENTRIONAL", "Argelia", "DZA", "012"),
                //new Country("AMERICA CENTRAL", "AMERICA CENTRAL", "---", "013"),
                //new Country("AFRICA ORIENTAL", "AFRICA ORIENTAL", "---", "014"),
                //new Country("AFRICA SEPTENTRIONAL", "AFRICA SEPTENTRIONAL", "---", "015"),
                //new Country("POLINESIA", "Samoa Americana", "ASM", "016"),
                //new Country("AFRICA CENTRAL", "AFRICA CENTRAL", "---", "017"),
                //new Country("AFRICA MERIDIONAL", "AFRICA MERIDIONAL", "---", "018"),
                //new Country("AMERICA", "AMERICA", "---", "019"),
                //new Country("EUROPA MERIDIONAL", "Andorra", "AND", "020"),
                //new Country("AMERICA SEPTENTRIONAL", "AMERICA SEPTENTRIONAL", "---", "021"),
                //new Country("AFRICA CENTRAL", "Angola", "AGO", "024"),
                //new Country("EL CARIBE", "Antigua y Barbuda", "ATG", "028"),
                //new Country("EL CARIBE", "EL CARIBE", "---", "029"),
                //new Country("ASIA ORIENTAL", "ASIA ORIENTAL", "---", "030"),
                //new Country("ASIA OCCIDENTAL", "Azerbaiyán", "AZE", "031"),
                //new Country("AMERICA SUR", "Argentina", "ARG", "032"),
                //new Country("ASIA MERIDIONAL", "ASIA MERIDIONAL", "---", "034"),
                //new Country("ASIA SUDORIENTAL", "ASIA SUDORIENTAL", "---", "035"),
                //new Country("AUSTRALIA Y NUEVA ZELANDIA", "Australia", "AUS", "036"),
                //new Country("EUROPA MERIDIONAL", "EUROPA MERIDIONAL", "---", "039"),
                //new Country("EUROPA OCCIDENTAL", "Austria", "AUT", "040"),
                //new Country("EL CARIBE", "Bahamas", "BHS", "044"),
                //new Country("ASIA OCCIDENTAL", "Bahrein", "BHR", "048"),
                //new Country("ASIA MERIDIONAL", "Bangladesh", "BGD", "050"),
                //new Country("ASIA OCCIDENTAL", "Armenia", "ARM", "051"),
                //new Country("EL CARIBE", "Barbados", "BRB", "052"),
                //new Country("AUSTRALIA Y NUEVA ZELANDIA", "AUSTRALIA Y NUEVA ZELANDIA", "---", "053"),
                //new Country("MELANESIA", "MELANESIA", "---", "054"),
                //new Country("EUROPA OCCIDENTAL", "Bélgica", "BEL", "056"),
                //new Country("MICRONESIA", "MICRONESIA", "---", "057"),
                //new Country("AMERICA SEPTENTRIONAL", "Bermuda", "BMU", "060"),
                //new Country("POLINESIA", "POLINESIA", "---", "061"),
                //new Country("ASIA MERIDIONAL", "Bhutan", "BTN", "064"),
                //new Country("AMERICA SUR", "Bolivia, Estado plurinacional de", "BOL", "068"),
                //new Country("EUROPA MERIDIONAL", "Bosnia y Herzegovina", "BIH", "070"),
                //new Country("AFRICA MERIDIONAL", "Botswana", "BWA", "072"),
                //new Country("AMERICA SUR", "Brasil", "BRA", "076"),
                //new Country("AMERICA CENTRAL", "Belice", "BLZ", "084"),
                //new Country("MELANESIA", "Islas Salomón", "SLB", "090"),
                //new Country("EL CARIBE", "Islas Vírgenes Británicas", "VGB", "092"),
                //new Country("ASIA SUDORIENTAL", "Brunei Darussalam", "BRN", "096"),
                //new Country("EUROPA ORIENTAL", "Bulgaria", "BGR", "100"),
                //new Country("ASIA SUDORIENTAL", "Myanmar", "MMR", "104"),
                //new Country("AFRICA ORIENTAL", "Burundi", "BDI", "108"),
                //new Country("EUROPA ORIENTAL", "Belarús", "BLR", "112"),
                //new Country("ASIA SUDORIENTAL", "Camboya", "KHM", "116"),
                //new Country("AFRICA CENTRAL", "Camerún", "CMR", "120"),
                //new Country("AMERICA SEPTENTRIONAL", "Canadá", "CAN", "124"),
                //new Country("AFRICA OCCIDENTAL", "Cabo Verde", "CPV", "132"),
                //new Country("EL CARIBE", "Islas Caimán", "CYM", "136"),
                //new Country("AFRICA CENTRAL", "Republica Centro  Africana", "CAF", "140"),
                //new Country("ASIA", "ASIA", "---", "142"),
                //new Country("ASIA CENTRAL", "ASIA CENTRAL", "---", "143"),
                //new Country("ASIA MERIDIONAL", "Sri Lanka", "LKA", "144"),
                //new Country("ASIA OCCIDENTAL", "ASIA OCCIDENTAL", "---", "145"),
                //new Country("AFRICA CENTRAL", "Chad", "TCD", "148"),
                //new Country("EUROPA", "EUROPA", "EUR", "150"),
                //new Country("EUROPA ORIENTAL", "EUROPA ORIENTAL", "---", "151"),
                //new Country("AMERICA SUR", "Chile", "CHL", "152"),
                //new Country("EUROPA SEPTENTRIONAL", "EUROPA SEPTENTRIONAL", "---", "154"),
                //new Country("EUROPA OCCIDENTAL", "EUROPA OCCIDENTAL", "---", "155"),
                new Country("ASIA ORIENTAL", "China", "CHN", "156"),
                //new Country("ASIA ORIENTAL", "Provincia China de Taiwán", "TWN", "158"),
                //new Country("AMERICA SUR", "Colombia", "COL", "170"),
                //new Country("AFRICA ORIENTAL", "Comores", "COM", "174"),
                //new Country("AFRICA ORIENTAL", "Mayotte", "MYT", "175"),
                //new Country("AFRICA CENTRAL", "Congo", "COG", "178"),
                //new Country("AFRICA CENTRAL", "República Democrática del Congo", "COD", "180"),
                //new Country("POLINESIA", "Islas Cook", "COK", "184"),
                //new Country("AMERICA CENTRAL", "Costa Rica", "CRI", "188"),
                //new Country("EUROPA MERIDIONAL", "Croacia", "HRV", "191"),
                new Country("EL CARIBE", "Cuba", "CUB", "192"),
                //new Country("ASIA OCCIDENTAL", "Chipre", "CYP", "196"),
                //new Country("EUROPA ORIENTAL", "República Checa", "CZE", "203"),
                //new Country("AFRICA OCCIDENTAL", "Benin", "BEN", "204"),
                //new Country("EUROPA SEPTENTRIONAL", "Dinamarca", "DNK", "208"),
                //new Country("EL CARIBE", "Dominica", "DMA", "212"),
                //new Country("EL CARIBE", "República Dominicana", "DOM", "214"),
                //new Country("AMERICA SUR", "Ecuador", "ECU", "218"),
                //new Country("AMERICA CENTRAL", "El Salvador", "SLV", "222"),
                //new Country("AFRICA CENTRAL", "Guinea Ecuatorial", "GNQ", "226"),
                //new Country("AFRICA ORIENTAL", "Etiopía", "ETH", "231"),
                //new Country("AFRICA ORIENTAL", "Eritrea", "ERI", "232"),
                //new Country("EUROPA SEPTENTRIONAL", "Estonia", "EST", "233"),
                //new Country("EUROPA SEPTENTRIONAL", "Islas Feroe", "FRO", "234"),
                //new Country("AMERICA SUR", "Islas Malvinas", "FLK", "238"),
                //new Country("MELANESIA", "Fiji", "FJI", "242"),
                //new Country("EUROPA SEPTENTRIONAL", "Finlandia", "FIN", "246"),
                //new Country("EUROPA SEPTENTRIONAL", "Islas Áland", "ALA", "248"),
                //new Country("EUROPA OCCIDENTAL", "Francia", "FRA", "250"),
                //new Country("AMERICA SUR", "Guayana Francesa", "GUF", "254"),
                //new Country("POLINESIA", "Polinesia Francesa", "PYF", "258"),
                //new Country("AFRICA ORIENTAL", "Djibouti", "DJI", "262"),
                //new Country("AFRICA CENTRAL", "Gabon", "GAB", "266"),
                //new Country("ASIA OCCIDENTAL", "Georgia", "GEO", "268"),
                //new Country("AFRICA OCCIDENTAL", "Gambia", "GMB", "270"),
                //new Country("ASIA OCCIDENTAL", "Estado de Palestina", "PSE", "275"),
                //new Country("EUROPA OCCIDENTAL", "Alemania", "DEU", "276"),
                //new Country("AFRICA OCCIDENTAL", "Ghana", "GHA", "288"),
                //new Country("EUROPA MERIDIONAL", "Gibraltar", "GIB", "292"),
                //new Country("MICRONESIA", "Kiribati", "KIR", "296"),
                //new Country("EUROPA MERIDIONAL", "Grecia", "GRC", "300"),
                //new Country("AMERICA SEPTENTRIONAL", "Groenlandia", "GRL", "304"),
                //new Country("EL CARIBE", "Granada", "GRD", "308"),
                //new Country("EL CARIBE", "Guadalupe", "GLP", "312"),
                //new Country("MICRONESIA", "Guam", "GUM", "316"),
                //new Country("AMERICA CENTRAL", "Guatemala", "GTM", "320"),
                //new Country("AFRICA OCCIDENTAL", "Guinea", "GIN", "324"),
                //new Country("AMERICA SUR", "Guyana", "GUY", "328"),
                //new Country("EL CARIBE", "Haití", "HTI", "332"),
                //new Country("EUROPA MERIDIONAL", "Santa Sede", "VAT", "336"),
                //new Country("AMERICA CENTRAL", "Honduras", "HND", "340"),
                //new Country("ASIA ORIENTAL", "Hong Kong (región administrativa especial de China)", "HKG", "344"),
                //new Country("EUROPA ORIENTAL", "Hungría", "HUN", "348"),
                //new Country("EUROPA SEPTENTRIONAL", "Islandia", "ISL", "352"),
                //new Country("ASIA MERIDIONAL", "India", "IND", "356"),
                //new Country("ASIA SUDORIENTAL", "Indonesia", "IDN", "360"),
                //new Country("ASIA MERIDIONAL", "Iran, República Islámica de", "IRN", "364"),
                //new Country("ASIA OCCIDENTAL", "Iraq", "IRQ", "368"),
                //new Country("EUROPA SEPTENTRIONAL", "Irlanda", "IRL", "372"),
                //new Country("ASIA OCCIDENTAL", "Israel", "ISR", "376"),
                //new Country("EUROPA MERIDIONAL", "Italia", "ITA", "380"),
                //new Country("AFRICA OCCIDENTAL", "Cóte D'Ivoire", "CIV", "384"),
                //new Country("EL CARIBE", "Jamaica", "JSM", "388"),
                //new Country("ASIA ORIENTAL", "Japón", "JPN", "392"),
                //new Country("ASIA CENTRAL", "Kazajstán", "KAZ", "398"),
                //new Country("ASIA OCCIDENTAL", "Jordania", "JOR", "400"),
                //new Country("AFRICA ORIENTAL", "Kenya", "KEN", "404"),
                //new Country("ASIA ORIENTAL", "República Popular Democrática de Corea", "PRK", "408"),
                //new Country("ASIA ORIENTAL", "República de Corea", "KOR", "410"),
                //new Country("ASIA OCCIDENTAL", "Kuwait", "KWT", "414"),
                //new Country("ASIA CENTRAL", "Kirguizistán", "KGZ", "417"),
                //new Country("ASIA SUDORIENTAL", "República Democrática Popular Lao", "LAO", "418"),
                //new Country("LATINOAMERICA Y EL CARIBE", "LATINOAMERICA Y EL CARIBE", "---", "419"),
                //new Country("ASIA OCCIDENTAL", "Líbano", "LBN", "422"),
                //new Country("AFRICA MERIDIONAL", "Lesotho", "LSO", "426"),
                //new Country("EUROPA SEPTENTRIONAL", "Letonia", "LVA", "428"),
                //new Country("AFRICA OCCIDENTAL", "Liberia", "LBR", "430"),
                //new Country("AFRICA SEPTENTRIONAL", "Jamahiriya Arabe de Libia", "LBY", "434"),
                //new Country("EUROPA OCCIDENTAL", "Liechtenstein", "LIE", "438"),
                //new Country("EUROPA SEPTENTRIONAL", "Lituania", "LTU", "440"),
                //new Country("EUROPA OCCIDENTAL", "Luxemburgo", "LUX", "442"),
                //new Country("ASIA ORIENTAL", "Macao (región administrativa especial de China)", "MAC", "446"),
                //new Country("AFRICA ORIENTAL", "Madagascar", "MDG", "450"),
                //new Country("AFRICA ORIENTAL", "Malawi", "MWI", "454"),
                //new Country("ASIA SUDORIENTAL", "Malasia", "MYS", "458"),
                //new Country("ASIA MERIDIONAL", "Maldivas", "MDV", "462"),
                //new Country("AFRICA OCCIDENTAL", "Mali", "MLI", "466"),
                //new Country("EUROPA MERIDIONAL", "Malta", "MLT", "470"),
                //new Country("EL CARIBE", "Martinica", "MTQ", "474"),
                //new Country("AFRICA OCCIDENTAL", "Mauritania", "MRT", "478"),
                //new Country("AFRICA ORIENTAL", "Mauricio", "MUS", "480"),
                //new Country("AMERICA CENTRAL", "México", "MEX", "484"),
                //new Country("EUROPA OCCIDENTAL", "Mónaco", "MCO", "492"),
                //new Country("ASIA ORIENTAL", "Mongolia", "MNG", "496"),
                //new Country("EUROPA ORIENTAL", "República de Moldavia", "MDA", "498"),
                //new Country("EUROPA MERIDIONAL", "Montenegro", "MNE", "499"),
                //new Country("EL CARIBE", "Monserrat", "MSR", "500"),
                //new Country("AFRICA SEPTENTRIONAL", "Marruecos", "MAR", "504"),
                //new Country("AFRICA ORIENTAL", "Mozambique", "MOZ", "508"),
                //new Country("ASIA OCCIDENTAL", "Omán", "OMN", "512"),
                //new Country("AFRICA MERIDIONAL", "Namibia", "NAM", "516"),
                //new Country("MICRONESIA", "Nauru", "NRU", "520"),
                //new Country("ASIA MERIDIONAL", "Nepal", "NPL", "524"),
                //new Country("EUROPA OCCIDENTAL", "Países Bajos", "NLD", "528"),
                //new Country("EL CARIBE", "Antillas Neerlandesas", "ANT", "530"),
                //new Country("EL CARIBE", "Curazao", "CUW", "531"),
                //new Country("EL CARIBE", "Aruba", "ABW", "533"),
                //new Country("EL CARIBE", "San Martín (parte holandesa)", "SXM", "534"),
                //new Country("EL CARIBE", "Bonaire, Saint Eustatius y Saba", "BES", "535"),
                //new Country("MELANESIA", "Nueva Caledonia", "NCL", "540"),
                //new Country("MELANESIA", "Vanuatu", "VUT", "548"),
                //new Country("AUSTRALIA Y NUEVA ZELANDIA", "Nueva Zelanda", "NZL", "554"),
                //new Country("AMERICA CENTRAL", "Nicaragua", "NIC", "558"),
                //new Country("AFRICA OCCIDENTAL", "Níger", "NER", "562"),
                //new Country("AFRICA OCCIDENTAL", "Nigeria", "NGA", "566"),
                //new Country("POLINESIA", "Niue", "NIU", "570"),
                //new Country("AUSTRALIA Y NUEVA ZELANDIA", "Islas Norfolk", "NFK", "574"),
                //new Country("EUROPA SEPTENTRIONAL", "Noruega", "NOR", "578"),
                //new Country("MICRONESIA", "Islas Marianas Septentrionales", "MNP", "580"),
                //new Country("MICRONESIA", "Micronesia, Estados Federados de", "FSM", "583"),
                //new Country("MICRONESIA", "Islas Marshall", "MHL", "584"),
                //new Country("MICRONESIA", "Palau", "PLW", "585"),
                //new Country("ASIA MERIDIONAL", "Pakistán", "PAK", "586"),
                //new Country("AMERICA CENTRAL", "Panamá", "PAN", "591"),
                //new Country("MELANESIA", "Papua Nueva Guinea", "PNG", "598"),
                //new Country("AMERICA SUR", "Paraguay", "PRY", "600"),
                //new Country("AMERICA SUR", "Perú", "PER", "604"),
                //new Country("ASIA SUDORIENTAL", "Filipinas", "PHL", "608"),
                //new Country("POLINESIA", "Pitcairn", "PCN", "612"),
                //new Country("EUROPA ORIENTAL", "Polonia", "POL", "616"),
                //new Country("EUROPA MERIDIONAL", "Portugal", "PRT", "620"),
                //new Country("AFRICA OCCIDENTAL", "Guinea-Bissau", "GNB", "624"),
                //new Country("ASIA SUDORIENTAL", "Timor Leste", "TMP", "626"),
                //new Country("EL CARIBE", "Puerto Rico", "PRI", "630"),
                //new Country("ASIA OCCIDENTAL", "Qatar", "OAT", "634"),
                //new Country("AFRICA ORIENTAL", "Reunión", "REU", "638"),
                //new Country("EUROPA ORIENTAL", "Rumania", "ROM", "642"),
                //new Country("EUROPA ORIENTAL", "Federación Rusa", "RUS", "643"),
                //new Country("AFRICA ORIENTAL", "Rwanda", "RWA", "646"),
                //new Country("EL CARIBE", "Saint-Bartolomé", "BLM", "652"),
                //new Country("AFRICA OCCIDENTAL", "Santa Elena", "SHN", "654"),
                //new Country("EL CARIBE", "Saint Kitts y Nevis", "KNA", "659"),
                //new Country("EL CARIBE", "Anguila", "AIA", "660"),
                //new Country("EL CARIBE", "Santa Lucía", "LCA", "662"),
                //new Country("EL CARIBE", "Saint-Martín (parte Francesa)", "MAF", "663"),
                //new Country("AMERICA SEPTENTRIONAL", "Saint Pierre y Miquelon", "SPM", "666"),
                //new Country("EL CARIBE", "San Vicente y las Granadinas", "VCT", "670"),
                //new Country("EUROPA MERIDIONAL", "San Marino", "SMR", "674"),
                //new Country("AFRICA CENTRAL", "Santo Tomé y Príncipe", "STP", "678"),
                //new Country("ASIA OCCIDENTAL", "Arabia Saudita", "SAU", "682"),
                //new Country("AFRICA OCCIDENTAL", "Senegal", "SEN", "686"),
                //new Country("EUROPA MERIDIONAL", "Servia", "SRB", "688"),
                //new Country("AFRICA ORIENTAL", "Seychelles", "SYC", "690"),
                //new Country("AFRICA OCCIDENTAL", "Sierra Leona", "SLE", "694"),
                //new Country("ASIA SUDORIENTAL", "Singapur", "SGP", "702"),
                //new Country("EUROPA ORIENTAL", "Eslovaquia", "SVK", "703"),
                //new Country("ASIA SUDORIENTAL", "Viet Nam", "VNM", "704"),
                //new Country("EUROPA MERIDIONAL", "Eslovenia", "SVN", "705"),
                //new Country("AFRICA ORIENTAL", "Somalia", "SOM", "706"),
                //new Country("AFRICA MERIDIONAL", "Sudáfrica", "ZAF", "710"),
                //new Country("AFRICA ORIENTAL", "Zimbabwe", "ZWE", "716"),
                //new Country("EUROPA MERIDIONAL", "España", "ESP", "724"),
                //new Country("AFRICA SEPTENTRIONAL", "Sur de Sudán", "SSD", "728"),
                //new Country("AFRICA SEPTENTRIONAL", "Sudán", "SDN", "729"),
                //new Country("AFRICA SEPTENTRIONAL", "Sahara Occidental", "ESH", "732"),
                //new Country("AMERICA SUR", "Surinam", "SUR", "740"),
                //new Country("EUROPA SEPTENTRIONAL", "Islas Svalbard y Jan Mayen", "SJM", "744"),
                //new Country("AFRICA MERIDIONAL", "Swazilandia", "SWZ", "748"),
                //new Country("EUROPA SEPTENTRIONAL", "Suecia", "SWE", "752"),
                //new Country("EUROPA OCCIDENTAL", "Suiza", "CHE", "756"),
                //new Country("ASIA OCCIDENTAL", "República Árabe Siria", "SYR", "760"),
                //new Country("ASIA CENTRAL", "Tayikistán", "TJK", "762"),
                //new Country("ASIA SUDORIENTAL", "Tailandia", "THA", "764"),
                //new Country("AFRICA OCCIDENTAL", "Togo", "TGO", "768"),
                //new Country("POLINESIA", "Tokelau", "TKL", "772"),
                //new Country("POLINESIA", "Tonga", "TON", "776"),
                //new Country("EL CARIBE", "Trinidad y Tobago", "TTO", "780"),
                //new Country("ASIA OCCIDENTAL", "Emiratos Árabes Unidos", "ARE", "784"),
                //new Country("AFRICA SEPTENTRIONAL", "Túnez", "TUN", "788"),
                //new Country("ASIA OCCIDENTAL", "Turquía", "TUR", "792"),
                //new Country("ASIA CENTRAL", "Turkmenistán", "TKM", "795"),
                //new Country("EL CARIBE", "Islas Turcas y Calcos", "TCA", "796"),
                //new Country("POLINESIA", "Tuvalú", "TUV", "798"),
                //new Country("AFRICA ORIENTAL", "Uganda", "UGA", "800"),
                //new Country("EUROPA ORIENTAL", "Ucrania", "UKR", "804"),
                //new Country("EUROPA MERIDIONAL", "Macedonia", "MKD", "807"),
                //new Country("AFRICA SEPTENTRIONAL", "Egipto", "EGY", "818"),
                //new Country("EUROPA SEPTENTRIONAL", "Reino Unido", "GBR", "826"),
                //new Country("EUROPA SEPTENTRIONAL", "Guernesey", "GGY", "831"),
                //new Country("EUROPA SEPTENTRIONAL", "Jersey", "JEY", "832"),
                //new Country("EUROPA SEPTENTRIONAL", "Isla de Man", "IMY", "833"),
                //new Country("AFRICA ORIENTAL", "República Unida de Tanzania", "TZA", "834"),
                //new Country("AMERICA SEPTENTRIONAL", "Estados Unidos de América", "USA", "840"),
                //new Country("EL CARIBE", "Islas Vígenes de los Estados Unidos", "VIR", "850"),
                //new Country("AFRICA OCCIDENTAL", "Burkina Faso", "BFA", "854"),
                //new Country("AMERICA SUR", "Uruguay", "URY", "858"),
                //new Country("ASIA CENTRAL", "Uzbekistán", "UZB", "860"),
                //new Country("AMERICA SUR", "Venezuela, República Bolivariana de", "VEN", "862"),
                //new Country("POLINESIA", "Islas Wallis y Futuna", "WLF", "876"),
                //new Country("POLINESIA", "Samoa", "WSM", "882"),
                //new Country("ASIA OCCIDENTAL", "Yemen", "YEM", "887"),
                //new Country("AFRICA ORIENTAL", "Zambia", "ZMB", "894"),
                new Country("------------------------------", "No Especificado", "---", "999")
            };
        }

        #endregion

        #region States

        private static List<State> GetInitialStates()
        {
            return new List<State>
            {
                new State("PINAR DEL RIO", "21", "Occidente"),
                new State("ARTEMISA", "22", "Occidente"),
                new State("LA HABANA", "23", "Occidente"),
                new State("MAYABEQUE", "24", "Occidente"),
                new State("MATANZAS", "25", "Occidente"),
                new State("VILLA CLARA", "26", "Centro"),
                new State("CIENFUEGOS", "27", "Centro"),
                new State("SANCTI SPIRITUS", "28", "Centro"),
                new State("CIEGO DE AVILA", "29", "Centro"),
                new State("CAMAGUEY", "30", "Centro"),
                new State("LAS TUNAS", "31", "Oriente"),
                new State("HOLGUIN", "32", "Oriente"),
                new State("GRANMA", "33", "Oriente"),
                new State("SANTIAGO DE CUBA", "34", "Oriente"),
                new State("GUANTANAMO", "35", "Oriente"),
                new State("ISLA DE LA JUVENTUD", "4001", "Isla"),
                new State("DESCONOCIDO", "88", "Desconocido")
            };
        }

        #endregion

        #region Cities

        private static List<object> GetInitialCities()
        {
            return new List<object>
            {
                new[] {"SANDINO", "2101", "21"},
                new[] {"MANTUA", "2102", "21"},
                new[] {"MINAS DE MATAHAMBRE", "2103", "21"},
                new[] {"VINALES", "2104", "21"},
                new[] {"LA PALMA", "2105", "21"},
                new[] {"LOS PALACIOS", "2106", "21"},
                new[] {"CONSOLACION DEL SUR", "2107", "21"},
                new[] {"PINAR DEL RIO", "2108", "21"},
                new[] {"SAN LUIS", "2109", "21"},
                new[] {"SAN JUAN Y MARTINEZ", "2110", "21"},
                new[] {"GUANE", "2111", "21"},
                new[] {"BAHIA HONDA", "2201", "22"},
                new[] {"MARIEL", "2202", "22"},
                new[] {"GUANAJAY", "2203", "22"},
                new[] {"CAIMITO", "2204", "22"},
                new[] {"BAUTA", "2205", "22"},
                new[] {"SAN ANTONIO DE LOS BANOS", "2206", "22"},
                new[] {"GUIRA DE MELENA", "2207", "22"},
                new[] {"ALQUIZAR", "2208", "22"},
                new[] {"ARTEMISA", "2209", "22"},
                new[] {"CANDELARIA", "2210", "22"},
                new[] {"SAN CRISTOBAL", "2211", "22"},
                new[] {"PLAYA", "2301", "23"},
                new[] {"PLAZA DE LA REVOLUCION", "2302", "23"},
                new[] {"CENTRO HABANA", "2303", "23"},
                new[] {"LA HABANA VIEJA", "2304", "23"},
                new[] {"REGLA", "2305", "23"},
                new[] {"LA HABANA DEL ESTE", "2306", "23"},
                new[] {"GUANABACOA", "2307", "23"},
                new[] {"SAN MIGUEL DEL PADRON", "2308", "23"},
                new[] {"DIEZ DE OCTUBRE", "2309", "23"},
                new[] {"CERRO", "2310", "23"},
                new[] {"MARIANAO", "2311", "23"},
                new[] {"LA LISA", "2312", "23"},
                new[] {"BOYEROS", "2313", "23"},
                new[] {"ARROYO NARANJO", "2314", "23"},
                new[] {"COTORRO", "2315", "23"},
                new[] {"BEJUCAL", "2401", "24"},
                new[] {"SAN JOSE DE LAS LAJAS", "2402", "24"},
                new[] {"JARUCO", "2403", "24"},
                new[] {"SANTA CRUZ DEL NORTE", "2404", "24"},
                new[] {"MADRUGA", "2405", "24"},
                new[] {"NUEVA PAZ", "2406", "24"},
                new[] {"SAN NICOLAS", "2407", "24"},
                new[] {"GUINES", "2408", "24"},
                new[] {"MELENA DEL SUR", "2409", "24"},
                new[] {"BATABANO", "2410", "24"},
                new[] {"QUIVICAN", "2411", "24"},
                new[] {"MATANZAS", "2501", "25"},
                new[] {"CARDENAS ", "2502", "25"},
                new[] {"MARTI", "2503", "25"},
                new[] {"COLON", "2504", "25"},
                new[] {"PERICO", "2505", "25"},
                new[] {"JOVELLANOS", "2506", "25"},
                new[] {"PEDRO BETANCOURT", "2507", "25"},
                new[] {"LIMONAR", "2508", "25"},
                new[] {"UNION DE REYES", "2509", "25"},
                new[] {"CIENAGA DE ZAPATA", "2510", "25"},
                new[] {"JAGUEY GRANDE", "2511", "25"},
                new[] {"CALIMETE", "2512", "25"},
                new[] {"LOS ARABOS", "2513", "25"},
                new[] {"CORRALILLO", "2601", "26"},
                new[] {"QUEMADO DE GUINES", "2602", "26"},
                new[] {"SAGUA LA GRANDE", "2603", "26"},
                new[] {"ENCRUCIJADA", "2604", "26"},
                new[] {"CAMAJUANI", "2605", "26"},
                new[] {"CAIBARIEN", "2606", "26"},
                new[] {"REMEDIOS", "2607", "26"},
                new[] {"PLACETAS", "2608", "26"},
                new[] {"SANTA CLARA", "2609", "26"},
                new[] {"CIFUENTES", "2610", "26"},
                new[] {"SANTO DOMINGO", "2611", "26"},
                new[] {"RANCHUELO", "2612", "26"},
                new[] {"MANICARAGUA", "2613", "26"},
                new[] {"AGUADA DE PASAJEROS", "2701", "27"},
                new[] {"RODAS", "2702", "27"},
                new[] {"PALMIRA", "2703", "27"},
                new[] {"LAJAS", "2704", "27"},
                new[] {"CRUCES", "2705", "27"},
                new[] {"CUMANAYAGUA", "2706", "27"},
                new[] {"CIENFUEGOS", "2707", "27"},
                new[] {"ABREUS", "2708", "27"},
                new[] {"YAGUAJAY", "2801", "28"},
                new[] {"JATIBONICO", "2802", "28"},
                new[] {"TAGUASCO", "2803", "28"},
                new[] {"CABAIGUAN", "2804", "28"},
                new[] {"FOMENTO", "2805", "28"},
                new[] {"TRINIDAD", "2806", "28"},
                new[] {"SANCTI SPIRITUS", "2807", "28"},
                new[] {"LA SIERPE", "2808", "28"},
                new[] {"CHAMBAS", "2901", "29"},
                new[] {"MORON", "2902", "29"},
                new[] {"BOLIVIA", "2903", "29"},
                new[] {"PRIMERO DE ENERO", "2904", "29"},
                new[] {"CIRO REDONDO", "2905", "29"},
                new[] {"FLORENCIA", "2906", "29"},
                new[] {"MAJAGUA", "2907", "29"},
                new[] {"CIEGO DE AVILA", "2908", "29"},
                new[] {"VENEZUELA", "2909", "29"},
                new[] {"BARAGUA", "2910", "29"},
                new[] {"CARLOS MANUEL DE CESPEDES", "3001", "30"},
                new[] {"ESMERALDA", "3002", "30"},
                new[] {"SIERRA DE CUBITAS", "3003", "30"},
                new[] {"MINAS", "3004", "30"},
                new[] {"NUEVITAS", "3005", "30"},
                new[] {"GUAIMARO", "3006", "30"},
                new[] {"SIBANICU", "3007", "30"},
                new[] {"CAMAGUEY", "3008", "30"},
                new[] {"FLORIDA", "3009", "30"},
                new[] {"VERTIENTES", "3010", "30"},
                new[] {"JIMAGUAYU", "3011", "30"},
                new[] {"NAJASA", "3012", "30"},
                new[] {"SANTA CRUZ DEL SUR", "3013", "30"},
                new[] {"MANATI", "3101", "31"},
                new[] {"PUERTO PADRE", "3102", "31"},
                new[] {"JESUS MENENDEZ", "3103", "31"},
                new[] {"MAJIBACOA", "3104", "31"},
                new[] {"LAS TUNAS", "3105", "31"},
                new[] {"JOBABO", "3106", "31"},
                new[] {"COLOMBIA", "3107", "31"},
                new[] {"AMANCIO", "3108", "31"},
                new[] {"GIBARA", "3201", "32"},
                new[] {"RAFAEL FREYRE", "3202", "32"},
                new[] {"BANES", "3203", "32"},
                new[] {"ANTILLA", "3204", "32"},
                new[] {"BAGUANOS", "3205", "32"},
                new[] {"HOLGUIN", "3206", "32"},
                new[] {"CALIXTO GARCIA", "3207", "32"},
                new[] {"CACOCUM", "3208", "32"},
                new[] {"URBANO NORIS", "3209", "32"},
                new[] {"CUETO", "3210", "32"},
                new[] {"MAYARI", "3211", "32"},
                new[] {"FRANK PAIS", "3212", "32"},
                new[] {"SAGUA DE TANAMO", "3213", "32"},
                new[] {"MOA", "3214", "32"},
                new[] {"RIO CAUTO", "3301", "33"},
                new[] {"CAUTO CRISTO", "3302", "33"},
                new[] {"JIGUANI", "3303", "33"},
                new[] {"BAYAMO", "3304", "33"},
                new[] {"YARA", "3305", "33"},
                new[] {"MANZANILLO", "3306", "33"},
                new[] {"CAMPECHUELA", "3307", "33"},
                new[] {"MEDIA LUNA", "3308", "33"},
                new[] {"NIQUERO", "3309", "33"},
                new[] {"PILON", "3310", "33"},
                new[] {"BARTOLOME MASO", "3311", "33"},
                new[] {"BUEY ARRIBA", "3312", "33"},
                new[] {"GUISA", "3313", "33"},
                new[] {"CONTRAMAESTRE", "3401", "34"},
                new[] {"MELLA", "3402", "34"},
                new[] {"SAN LUIS", "3403", "34"},
                new[] {"SEGUNDO FRENTE", "3404", "34"},
                new[] {"SONGO - LA MAYA", "3405", "34"},
                new[] {"SANTIAGO DE CUBA", "3406", "34"},
                new[] {"PALMA SORIANO", "3407", "34"},
                new[] {"TERCER FRENTE", "3408", "34"},
                new[] {"GUAMA", "3409", "34"},
                new[] {"EL SALVADOR", "3501", "35"},
                new[] {"MANUEL TAMES", "3502", "35"},
                new[] {"YATERAS", "3503", "35"},
                new[] {"BARACOA", "3504", "35"},
                new[] {"MAISI", "3505", "35"},
                new[] {"IMIAS", "3506", "35"},
                new[] {"SAN ANTONIO DEL SUR", "3507", "35"},
                new[] {"CAIMANERA", "3508", "35"},
                new[] {"GUANTANAMO", "3509", "35"},
                new[] {"NICETO PEREZ", "3510", "35"},
                new[] {"ISLA DE LA JUVENTUD", "4001", "4001"},
                new[] {"DESCONOCIDO", "8888", "88"},
            };
        }

        #endregion
    }
}
