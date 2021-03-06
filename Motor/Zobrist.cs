﻿namespace Enxadrista
{
    /// <summary>
    ///     Gera chave zobrist para a posição.
    /// </summary>
    /// <remarks>
    ///     Método Zobrist
    ///     --------------
    ///     Este componente é responsável por gerar uma chave "única" para uma
    ///     posição de xadrez. Essa chave é gerada usando o método Zobrist.
    ///     Na realidade, não podemos ter, de uma forma prática, uma chave única
    ///     para todas as posições de xadrez, mas esse método nos permite gerar
    ///     uma chave que seja útil para um motor de xadrez.
    ///     É possível ter colisões de chave, mas esse risco é mínimo e aceitável
    ///     para o motor de xadrez.
    ///     A chave da posição será usada para detectar a repetição da posição,
    ///     como na regra de três repetições, mas mais importante para a tabela
    ///     de transposição, que requer uma chave única para a posição.
    ///     Functionamento
    ///     --------------
    ///     Basicamente, temos um número aleatório para cada peça, em cada quadrado
    ///     para cada cor que são combinados usando o operador lógico XOR, chamado
    ///     "ou exclusivo".
    ///     Também incluímos outras informações, como cor, estado roque, etc.
    ///     E teremos um número de 64 bits que é mais ou menos único para a posição.
    ///     Veja o método abaixo para mais detalhes.
    ///     Origem dos números aleatórios
    ///     -----------------------------
    ///     Os números aleatórios abaixo vêm do meu outro motor Tucano. Você pode gerar
    ///     esses números usando a função "random()". Mas se você tiver o conjunto
    ///     completo pré-gerado, você sempre obtém os mesmos resultados toda vez que você
    ///     executa seu programa em computadores diferentes. Digamos que isso seja uma coisa
    ///     aleatória a menos no seu motor, que já é muito complexo.
    /// </remarks>
    public class Zobrist
    {
        /// <summary>
        ///     Gera a chave para a posição atual do tabuleiro.
        /// </summary>
        /// <remarks>
        ///     Aqui estamos gerando a chave para a posição sempre que tivermos uma nova posição.
        ///     A maioria dos programas irá fazer isso de forma incremental, apenas atualizando
        ///     os elementos que mudaram quando o movimento foi feito, exemplo a peça do movimento.
        ///     Isso é possível por causa da forma como o "ou exclusivo" funciona.
        /// </remarks>
        /// <param name="tabuleiro">Tabuleiro.</param>
        /// <returns>Chave da posição.</returns>
        public static ulong ObtemChave(Tabuleiro tabuleiro)
        {
            ulong chave = 0;

            for (var fileira = Defs.PRIMEIRA_FILEIRA; fileira < Defs.ULTIMA_FILEIRA; fileira++)
            for (var coluna = Defs.PRIMEIRA_COLUNA; coluna < Defs.ULTIMA_COLUNA; coluna++)
            {
                var indice12x12 = fileira * Defs.NUMERO_COLUNAS + coluna;
                var peca = tabuleiro.ObtemPeca(indice12x12);
                if (peca == Peca.Nenhuma) continue;
                var indice8x8 = Defs.Converte12x12Para8x8(indice12x12);

                switch (peca)
                {
                    case Peca.PeaoBranco:
                        chave ^= Chave.Branco.PEAO[indice8x8];
                        break;
                    case Peca.CavaloBranco:
                        chave ^= Chave.Branco.CAVALO[indice8x8];
                        break;
                    case Peca.BispoBranco:
                        chave ^= Chave.Branco.BISPO[indice8x8];
                        break;
                    case Peca.TorreBranca:
                        chave ^= Chave.Branco.TORRE[indice8x8];
                        break;
                    case Peca.DamaBranca:
                        chave ^= Chave.Branco.DAMA[indice8x8];
                        break;
                    case Peca.ReiBranco:
                        chave ^= Chave.Branco.REI[indice8x8];
                        break;
                    case Peca.PeaoPreto:
                        chave ^= Chave.Preto.PEAO[indice8x8];
                        break;
                    case Peca.CavaloPreto:
                        chave ^= Chave.Preto.CAVALO[indice8x8];
                        break;
                    case Peca.BispoPreto:
                        chave ^= Chave.Preto.BISPO[indice8x8];
                        break;
                    case Peca.TorrePreta:
                        chave ^= Chave.Preto.TORRE[indice8x8];
                        break;
                    case Peca.DamaPreta:
                        chave ^= Chave.Preto.DAMA[indice8x8];
                        break;
                    case Peca.ReiPreto:
                        chave ^= Chave.Preto.REI[indice8x8];
                        break;
                }
            }

            if (tabuleiro.IndiceEnPassant != 0)
                chave ^= Chave.ENPASSANT[Defs.Converte12x12Para8x8(tabuleiro.IndiceEnPassant)];

            chave ^= Chave.Roque.E1G1[tabuleiro.RoqueE1G1 ? 0 : 1];
            chave ^= Chave.Roque.E1C1[tabuleiro.RoqueE1C1 ? 0 : 1];
            chave ^= Chave.Roque.E8G8[tabuleiro.RoqueE8G8 ? 0 : 1];
            chave ^= Chave.Roque.E8C8[tabuleiro.RoqueE8C8 ? 0 : 1];

            if (tabuleiro.CorJogar == Cor.Preta) chave ^= Chave.COR;

            return chave;
        }

        /// <summary>
        ///     Contém todos os números aleatórios necessários para a geração de chaves.
        /// </summary>
        public class Chave
        {
            public const ulong COR = 0x6CA3EC67E69AA24C;

            public static readonly ulong[] ENPASSANT =
            {
                0xB366D1A55AC34195, 0x60D3FCFBCF572684, 0x672F9B58D7DFE3F8, 0xE09ACCE13480CBCE,
                0xFEA61BE2004B3621, 0xD169BC626DC8EEFF, 0xFF7171B1307B257B, 0x637CD3DB2060DBAD,
                0xE66D5CBF3C16EA7E, 0x2827BBFEDEC37D56, 0xC5981B79FA1AB2C5, 0x5673E0CE3F240744,
                0x8E4027DB198906C4, 0x9AE81971F49F1693, 0xE69D57E2972CBABF, 0x628C4CEE35D5D1FA,
                0xAAE26EBFBB93B102, 0xB5087E6DD9D2CC02, 0xD3045080B005435F, 0xB0C13739F723FFCD,
                0xD1D3ED38A1D1E480, 0x88229C3D8143F5A4, 0x69B7BE0DBD0B7D91, 0x1AC995496580C3DF,
                0xF131FABEAA3FCB47, 0x020D7918CABA89E4, 0x6F9044BA9680955A, 0x686BC6695E677E50,
                0x228269EF09979DE0, 0x28A4C4488D74068A, 0x8A03D90F93EDA238, 0xADDBA608B0B9DD4D,
                0x42E64F15944A4D5A, 0x33DAB61D5AD25EED, 0xBB7AAE5DE5299BCC, 0x379D4A75E047F566,
                0xF333B6C452177B90, 0x268EF0B2AB30236E, 0x77CF7FBFAF0576BC, 0x48728CEBB86BA525,
                0x1A78857E764B85A9, 0xBEB0868586D2D622, 0x1F7494A9D5954DD4, 0x05CF67EE0FD157DD,
                0xDD65896EF696A4E1, 0xBD259BD89CFAA0CF, 0xC7C3320D3E1C3B6D, 0xB75668F7BF537BC1,
                0x35201733DA886988, 0x3DFBE7E95B17601B, 0x2EF17906F293C711, 0x25DCE275AE00BF36,
                0xD7FCDAB63DA69449, 0x0161CAF4B6173800, 0x3231842170D70B5C, 0xCE781B16B43E696B,
                0xBEA2B5125E2C91AE, 0xADE63E0CF7DC2584, 0xC47784260F6E7826, 0x0610C765B8066228,
                0x2814D09272609BE1, 0xAC8151BD42CBC1A8, 0xBC680D7DC1F5D4E0, 0x8200AAB8044DBCE4
            };

            public class Roque
            {
                public static readonly ulong[] E1G1 = {0x4CB1297B347D9184, 0xDAF24440D43B9E43};
                public static readonly ulong[] E1C1 = {0x0981309987EC4A67, 0x0D7D7C634883A7BB};
                public static readonly ulong[] E8G8 = {0x50DAC80A753049B4, 0xD2203D893B0399E6};
                public static readonly ulong[] E8C8 = {0xD6AB15746539DAC2, 0x43EC3CFA891AE17D};
            }

            public class Branco
            {
                public static readonly ulong[] PEAO =
                {
                    0x152A58F9E889607C, 0x2F2810AF5A1A8312, 0x69F12DA056E9FE7B, 0x2A23B15BCB715063,
                    0x9BAF26235CA877B8, 0xC0E34F0ECE76631F, 0x0133097D088D52B8, 0x5794D68A7EF654FA,
                    0xB35C1658CC6CF213, 0x2F2D90978305BAD4, 0x03202519543A6693, 0xF9E022EDC7D3EFAD,
                    0x16FBD14408D3757E, 0xEA3DB4FBF48ADCE1, 0x89DDA6437140669C, 0x6F0F19E2026ED596,
                    0xC96D1E4203356EAE, 0xF723E57250EDAB85, 0x308FAD33D6D01E78, 0xB3B5B524F515DD83,
                    0x575AC9E7A19D738C, 0xC7A7DB76D5693473, 0x5D261F5CA221CF72, 0x3FD6C840C5BAE186,
                    0x3E3CBD56073782EA, 0x7ECF5F1CB253C8F8, 0x56B0B48465B29651, 0x9EAD5205A64AA6AF,
                    0xE91FBC60C2E45578, 0x488D0DEDBE88B65C, 0x9CBBC33423AAB46A, 0x2BDED51011AA0F15,
                    0x3BB6AF8235EA33F6, 0xC3166B5CE4738286, 0x2C34B974555BCFF7, 0x0896A065534D9A1C,
                    0x8C32CE9FA4C806AC, 0xC0630BC692BA50E4, 0xA6567ED9C1E57FA7, 0xA307E119B375F6F3,
                    0x2463B1A044269218, 0x3E523B021A83D38B, 0xA61DCCE285F63578, 0xB2D8060A82062D5D,
                    0x4AA09BD0F3EA94E8, 0xCBFA2F81C567EC9B, 0x18C65AA30EAE5CC2, 0xA4F16EA85A0120B3,
                    0xAAF84718BA67BB28, 0x8A9E4B050CF3E3E6, 0x15CE68C2E9A03F95, 0xA93E0DCD57A37127,
                    0x712A23FEB1B09ABD, 0x00CD79D87DD6D4D2, 0xBB042ACA2BF40745, 0x2CC999A89B1A7F4B,
                    0xBBE6337EF90B2D13, 0x9E28B1A6F0ACB87D, 0xE919BABDD2A96C44, 0xF2CC11B5D1E015D2,
                    0x89D56FAF9C7FFA13, 0x7651E0DB62663020, 0xB541540DDDF7E931, 0x771C93C720B77D96
                };

                public static readonly ulong[] CAVALO =
                {
                    0x68EB0834C48F6859, 0x61820F91565373B7, 0xCC52E2D0C9D2852F, 0x1C8B891D6840FFDC,
                    0x587C6787A00066A5, 0x7947A315CBCE08DF, 0x6BF78452A58A8979, 0x41A56491FE3A1AD5,
                    0x36A1660919D5628F, 0x8FED64F87184A600, 0xCE5E65EF2C903633, 0xDE5CFACED553EC66,
                    0xF95D9FE80D5BC080, 0x7908FEABE06265AC, 0x6CF91F416252CA84, 0x451DAD986DAD9729,
                    0xDC1012D68A6048DE, 0x71B06AB808192E48, 0x16C0749FD34773E2, 0x09C6B32587F0A5B4,
                    0xA1AC588E5B7E3E88, 0x2EDA6C79D74EF1EB, 0x8E80BEE8BD05F8AE, 0x990D328A3508301B,
                    0x143A6325E0951282, 0x10632F758962AA84, 0x3DAE259FBF8F3202, 0xE2C65E2E7C7B47B2,
                    0xE2201035AF56E8F2, 0xBF3B9F35E3DAB640, 0xE041885B19ACA8C8, 0x819A655873686D15,
                    0x31B077CC8BFB634C, 0xA54018BF0D6CF82B, 0xD61CE48092726A0C, 0xE29F45C67E1E146A,
                    0x9B850C35EC1463CB, 0x3536468E88A76117, 0xDE77B45151E1E694, 0xA75A76555656FBE6,
                    0x941E6F8CD57BBE21, 0x7174482C7C3CC4BB, 0xE9D31448CAABF1AE, 0xB4ABF3BC5010D88F,
                    0xB54BF4238B690970, 0x57AEB64B92EE1F1D, 0x06F70BC87711B349, 0xC9208853F7081D41,
                    0xDFDD4CB65BA18779, 0x206F647D1D132E32, 0xB776A6144AE8A149, 0x5D2CD7EFBED2DFF5,
                    0x8619E1713DC9D513, 0xB8B3D16E3DC503C4, 0x303C98A21CBAD415, 0x07CEC8C7DF98A13E,
                    0xF7826EC516D9D7E0, 0x943386BACDA9F395, 0xC29F23B3E8094A73, 0xFA193271196EFA16,
                    0x9E58630D52AC1D3D, 0x6ECEF94F7D5A81C3, 0x927D6A462AAE7096, 0xBC258BE6785073D8
                };

                public static readonly ulong[] BISPO =
                {
                    0x176FA00325B31E78, 0xFBA54B582173B56C, 0xA5E3CE4CFF5EFE76, 0xEDEEE1A1D2B90E8E,
                    0xB4C5D503E8C81E46, 0xA853C0C2B2387393, 0x72B78E40774D8160, 0x5693ECC59CE0396E,
                    0x93639726211F1375, 0x92DB874FE8DFC644, 0x86EC90FA14ECA4CF, 0x0481F454CA90C1A0,
                    0xE504AC1DE657D4DF, 0xF8B0A333447F33F4, 0x7FC11CE1E9CE8E7B, 0xC907317C99A3FB43,
                    0x3608ECF1F8B0FA9F, 0x2D70214660948B2C, 0x7C82276B39A961B0, 0x21CD3FED7F1CA8B0,
                    0x802C1580F957A889, 0x2FBEBFC6A72DC666, 0x6158D9E0E4773DE2, 0x1ABB734350E063FE,
                    0xC28705D696DCB5D4, 0x8ACF0EA8C8B56C38, 0x069D988479BB4C7F, 0x9AC1BA80260F8EC7,
                    0x0F57AA505FC4E61A, 0x1B10D879C35A0FB1, 0x6935E8F8B6DFB205, 0x860AEA92C8048C2A,
                    0x8811EF920D3AA784, 0xD486D4CE241CD501, 0xE878450012BC7654, 0xA81758F26DE6C912,
                    0x3837B74BF7E00A3F, 0x314535440B759E5D, 0xC618768958333847, 0xC03EDF451CEAD6B6,
                    0xD779F2CE48C36533, 0x76986E14DFA14523, 0x149C2A0C71F8407F, 0x231C311AE7279B62,
                    0x23A0B870586BDBF5, 0xE753EB35368A033A, 0x38D85437E96E4780, 0xAB5E9CB523155D76,
                    0x6ABBF8C2CE1139FB, 0x01C7B305A15270BD, 0xDAF417ED7AAC0300, 0xFC84FFE0D5A2ACAD,
                    0x261BC897B0EB570F, 0xEFDFE795637D39DA, 0x7B76698EBF9ECE7C, 0x11FFE9DF28F26DA1,
                    0x1E9144D47DA27907, 0x67EA8771BFB9F0F5, 0x88DABC9B5448830D, 0x5D3CD35D69B20B8C,
                    0xBA76C518F1E4E6B3, 0x54867F07244D2112, 0xBA2C389CF625D27B, 0x8D17857FE5181451
                };

                public static readonly ulong[] TORRE =
                {
                    0x1E000233E0152018, 0xC2398B93611B8472, 0x0B2B826400AE3990, 0xA13598F9A17AC2BB,
                    0xAE53A567BD206FDD, 0x592F80A4F153037C, 0x4E7B169648F65DAF, 0x83C846358B893B06,
                    0xCFEA657FFA6B4A04, 0x719FF12D82B5DFE0, 0x3252E28A7A70E9A6, 0xAC376F901229FD9D,
                    0xF0B906B55DE82BDB, 0x895D2D2284822FF8, 0xBC3DB874B6D3BDC1, 0xE533BE9F0AF05423,
                    0xE69F6263EA47F132, 0x290B14AD27FF8E6F, 0x485D0BE59B160721, 0x38B11D8C5938BCB5,
                    0xF2A22B8F4948D2CF, 0x9F7C91EC9CA6A226, 0xF0B67395AB95924D, 0xAC49857CF1E2976F,
                    0x416A75380A2E821F, 0xB1BB3A4731EB6254, 0xD9073D828955ED06, 0xA0878B0AF5A99A2E,
                    0x398EAF80E64F1630, 0x8634C84968A7BAEE, 0x13A0F25CB2642B61, 0x3D9861CF7C1FCE95,
                    0x202320A25DC7BDDD, 0x4992AF199E24F547, 0x7BCE9542EF596410, 0x5FE6DFF949481E54,
                    0x74128EB19E4CEB49, 0xA3B7B8771AC588F8, 0x6A40BCD1AFFA3801, 0x1F1D3AF8C7D0B1A5,
                    0xF8BE3F34861C6C8F, 0x3B66CE4C2E513903, 0x4001927D78FBF92C, 0xF210C63641EB2418,
                    0xB066A186B51225B8, 0x7F11A0CDE3DBA134, 0x9C721541E4E3C4A6, 0x1A136CDCB2C95391,
                    0x3EDC690E7DD75FEE, 0x214F2A2EF75117C9, 0x6ACE4C9D9A0B23FB, 0x782E6FB16AB34D90,
                    0x64F80C41E93461F0, 0xAF79A0DE569E8754, 0x6BB9C8DFB7BF89BB, 0x46C51FFE5FC61EB1,
                    0x0D5B2D7C5D8545C6, 0xF0F5B95B187DA4DD, 0x57543DC41C86BD53, 0x1A1EC08AE54A0874,
                    0xEAF842A21C493FB7, 0x45A0A89272DD264D, 0x0C555461C77E8A1E, 0x025A65A363AEE13F
                };

                public static readonly ulong[] DAMA =
                {
                    0x61E4409767D5CA7A, 0xD1E707D31AEB5D08, 0xC8B26166B6E0C5BD, 0x3147DD340B2422A3,
                    0x2AEDBF834328ECA9, 0x9F02EB4C98C058D9, 0xB04A3CA4C5A301A5, 0xEEAA97F15AD4E9DC,
                    0xCA7E3CE65DD98B7C, 0x2DE24C2238A7F714, 0xDE1926EFEA3C7BF8, 0xAD655C9036C06E9C,
                    0x0B42B07E9E2D7EB4, 0x13367709240A81F8, 0x3476BB499881A397, 0xAC0B1E0FA633B409,
                    0xE51BD8FC9B4459D6, 0x7B29007C8BFA4453, 0x98D8F85F68AEC375, 0x335A5E1234E5F805,
                    0xF0D6958979721E9A, 0x9B3C2F7C6F59356A, 0x4FA1BF568581292F, 0x7D1F944610AFA4AF,
                    0xA12C911ABAAACCA4, 0xB0CF5FDFA6A52119, 0x8472B4E9C37FDADA, 0x25FFC3DEDDE7C827,
                    0x718CD49735160272, 0x02CF4D347F627037, 0xF38A41D6A359B01C, 0x84AAB81DDC59D697,
                    0x232C44CD41C32397, 0xF50BC7319124753D, 0x70A7FF992169627F, 0xE8F590EB96E23A70,
                    0x38DDC13755793827, 0xA3B5E0B796376127, 0xCFF88E7A1455E903, 0x8D5D7880F7A9F6F2,
                    0xDA33488F95A7F973, 0xB5877A61F1ECC49C, 0x5D8AC1EBBCD632F4, 0x2D71EB1F3AFBE0F0,
                    0x6E65F836DB7CBFF8, 0x0B1039A66E82C14A, 0x27CC6036089215FB, 0x2BA749DA1AC3B1D3,
                    0x6786AB68B7134692, 0x89ADCD885CAF4F95, 0x698DA2752A248378, 0xD9116170ECA478DD,
                    0x3E768F4024C5F6F6, 0xE8A911D8DEC9BC72, 0x080ED1E4883DD910, 0xA9861E35CCB4AEB0,
                    0xC827B08CC6A2335F, 0x5EFD1206C995438E, 0x0193AF7E25E46083, 0x46A756090ED6EF0F,
                    0x34A5387953FA5189, 0x3845BC8124AAE9B4, 0x810102E505D8D2C1, 0x2C562160D0B39CE4
                };

                public static readonly ulong[] REI =
                {
                    0x00643CFDD1143ADC, 0x3F55C7A77482FB6E, 0x03005DA46B16723E, 0xB10D4C61E656F484,
                    0x385BFA27010165EB, 0x54F5F94A181ED3EB, 0xE12CAFBB78759083, 0x64A2D9097B635030,
                    0xA966B929E38A1D1D, 0x814AF1BD24555021, 0x6FC77A7A916E1306, 0x07F39B6562F2A2DD,
                    0x3B699A49B145C6AA, 0xB6642B7746B89234, 0xE2745EB0FDF94B3C, 0x17F548DE700D6D36,
                    0x40C5E68C67C7A1CB, 0xF27250449A247917, 0x447DD328B391B5ED, 0x0EB0E38F10C330DF,
                    0x9A913E3F57F3AD2B, 0x18255505B6E57073, 0x39226073C559F5C8, 0x54A57AAF85E7A5FC,
                    0xDA15694D8870AAA1, 0x6BB3DD02E882FCC5, 0x40684F0A9D589D38, 0xF512690BF16AB6F3,
                    0xC01A5E631E38F422, 0x7B0895CAD32AA7CC, 0x14FA44B700E01B77, 0x7FA9EA8E5552066F,
                    0x727505E31D4C95F2, 0xFF9A0CA632BB3123, 0xFF8C7E52820C60E1, 0x5A2BD1D8FD5503A7,
                    0x98614DACB385E21B, 0x2067CE95FD6AA72B, 0xC447C2D2E0668E8C, 0x1C67B6ECED1550DD,
                    0x962132AF5F85F121, 0x02A213E614144C2C, 0x66C03CA73AA84517, 0xB92767E4CCF8802A,
                    0x18669550E5CBC3F5, 0x9F77684E4A1FEBBD, 0xE8EF5664939A66C0, 0xEE81F4BDEFA4EA7A,
                    0xF802BDA1A5E57B28, 0x3D8B90A2AF0D0462, 0xD1BA27C3CC19E0B9, 0x2015DF2F6A1624D4,
                    0x965F4461C1BF5761, 0xA4961F14E39EE974, 0x5F7D4FEE14394FBD, 0x15AAE795585DD7DD,
                    0x5B3EE5D6A919F712, 0x69A8970564A0C147, 0xB3214A19758093DF, 0xC0BE3FE8C8F7FD9C,
                    0x9445536E7B168567, 0x3A9D316395560387, 0x38375976B853A7A4, 0xF372FDC8A8C7C980
                };
            }

            public class Preto
            {
                public static readonly ulong[] PEAO =
                {
                    0xFEC7E9370FF14E80, 0xC72D82A03381DBE4, 0xB69D396D866F4E52, 0x1B6E5492A9B1BEA0,
                    0xDC669C225CC9ECE1, 0x9E30B32D3F0F14F0, 0xD22ACF2A4C8D6D87, 0x0B18F08D7DD3B842,
                    0x4AEBE61A039D0428, 0xBD81AB8EBF5E6F46, 0xE4E5C97AFD27A60E, 0xC3CA5C1FA16268A0,
                    0x7FF5C4E68152BCFE, 0x4B0EA4FD302C08ED, 0xC83E6BFAB659FFF0, 0x2B5A681ACD1D91E8,
                    0xF0E5F0E2CBF0F651, 0xC58E9F95001E38FC, 0x79D3B2912CE0BDC7, 0xCF9CE0134B717D83,
                    0xB79A008152ED5FC1, 0x13602F18AAEA837F, 0xE040AA3D5E3FAB58, 0x624343C90A2A7598,
                    0xDC6D3DA1B3A0F158, 0xEE105F405923899A, 0xEC6FEC34AC00FB61, 0x3FA76EA1E2D244D1,
                    0x25FF01B513D7F47F, 0xF108609D0FA099EE, 0x6BF8D84DE21788B1, 0x1AFE063133A9005D,
                    0x1745BDB3F484922F, 0x39E57F075889C53A, 0xE903DBBF4A628E77, 0xA27198D103402037,
                    0x8C6502E1EE921A64, 0x20F552A1AFFF9B42, 0x8C36A12C0F4CABDC, 0xC5A3A54CC8B329A7,
                    0x3ECFD1642BD578D8, 0x185EEC68DE6773F5, 0x6D2996FFCF9146D5, 0xB119549792827007,
                    0xE13061A4CE1E8AEF, 0x386F2F55CA5268CC, 0xE5E2A91D651B2233, 0x490B9E97660C2FC8,
                    0xCB988D89966DCAEE, 0x2790770D6C0B626E, 0xB5DB9AD9C109BAFE, 0x562382FCD1A887B5,
                    0x887A6E77BE410472, 0xE468DD22ACBD5E9B, 0x690D5E4BDED14101, 0xBE9BAF2CCB619478,
                    0xCAE91528D50A861D, 0x40A03DE65B3F5248, 0x5F84F5E6307CBEA6, 0xCE4B7A39564DB25B,
                    0x0EA0B05113BF8493, 0x32CF6DC9807F4C07, 0x2A001D6686104405, 0x58188FEA088A2951
                };

                public static readonly ulong[] CAVALO =
                {
                    0x4A576713788BA2A5, 0xAE16804DBD883AAB, 0xE8112712120AA939, 0x635293D6ABD54736,
                    0x14D57C459CA2D2CD, 0x55DB1185BD30FC2A, 0x6C4C8A41CD09BBF3, 0xCF74F58D52C72C53,
                    0xC555FF87163185DB, 0xAC2EB326E36318C3, 0x62FC30405A886C52, 0xF0D17ECF1CADCE26,
                    0xDFAF41255873DAEF, 0xE15DCE2A4405CE61, 0xDFDD1C76B0C34CF3, 0xD0A294D58204F461,
                    0xF4C38DD023E044A8, 0xD32619FE0B87D041, 0x1F62BCE8E5BA7A45, 0x630675AE63903432,
                    0x87B9471EF87F579C, 0x9214BA44E309DED0, 0x5004600363523D20, 0x9D610AA2B6172DC3,
                    0xA07BBDE65C5B3608, 0x2B8D0428FA281BD8, 0x70119CB7E4987C96, 0xDFA6F8B104BE1001,
                    0xAC03235CE812CCCA, 0x7EF71C3AC064BEDC, 0xE48DC9EAFE20D30A, 0x5B0F2815E3FEA6A2,
                    0x12E6160F6B8AD58C, 0xBC95EDE538300DC0, 0x4596522F938C5A80, 0xD8AE8BE48F447662,
                    0x16B11FA71CBF5243, 0xB4868D6AED963DAE, 0xF4CD20D5BC280434, 0x9A79A2B0CB242B8F,
                    0x9E87487DA2B775D9, 0xE465E57A7A85A037, 0x6A4EF54638B10F6F, 0x092F99478638DEC6,
                    0xC38B96026E95AE27, 0xDA1FA14CE2BA40B6, 0x1CAE99B0BC357396, 0x59AC2877129AF3FC,
                    0xF59018F074CBC326, 0x5A649458F14855FB, 0x107A0A076A155185, 0x6C22B6E9B1FB97C1,
                    0x188FC051386ABF67, 0xB1491091A1C52C26, 0x23C6C84DCB26C01D, 0x7FC0780C8E5F9EC1,
                    0x036DF6543A9615C2, 0xFB8BEC39FF11EAD1, 0xE2468B341BF78819, 0x1D37110B1977CB8C,
                    0x807F7BF1C014B84C, 0x3F04484301C5DB73, 0x36634404F62D2326, 0x62AF1CD5EC9B3E99
                };

                public static readonly ulong[] BISPO =
                {
                    0xE85AA8623F04948C, 0xD0B8B83F98361402, 0x73E428E1F7099233, 0x83A2583D63611486,
                    0xE770A16066AB28EE, 0xAF1D1CE3A22429D8, 0xB099A051650BDF07, 0xB71EEF198ADD76A2,
                    0x26ECD1411368A778, 0x5EF9B2158A0350D8, 0xD792BB1B36AFDD12, 0x86F0F3835F76A1AE,
                    0x395E0FD620C8D9BD, 0x8179818F45E48AD3, 0x2B583974FC55FD86, 0x3A35781E736226E1,
                    0x43A703242BB53A11, 0x21E0DB0FEA00CB26, 0x33B2A87FCF3ECAA6, 0xEDD55670B7BFEB2A,
                    0xD2B6CCE876C8F5FC, 0xA16AE31AD6E2B2A5, 0x6B75B51587B2DE5F, 0xE566BA4C424F28BB,
                    0x4688E9EBC2BFCDF2, 0x2FD1F94BAD30C7BD, 0x0AD520E49C3EE816, 0xE6F8DB4909CEF6C4,
                    0x32EE982A4D098641, 0xF6FC27341115CED6, 0x92C176DE6C1A95C2, 0x3C453D4CB2F31B7D,
                    0x5E9E48C5867EDD4B, 0x785398CF674E26F7, 0x15CBCBF2D6A8F43C, 0xFBCB01232800AC68,
                    0x210D52CBF42BBAF7, 0xB2436F81D5CECEAF, 0x1313091F62181AD3, 0x94519E29150740D5,
                    0xD78F8BCBCC4B0D62, 0x7C5F10AA720FF530, 0x4FB84BC993283624, 0xDD524E0482BBBAA7,
                    0xB23FFB39C15086DC, 0x70B0C0C3A6F83BBE, 0x735B196DF8084629, 0xF6CB836D29EFE956,
                    0x71316DA5CD203110, 0xD0AFCC14FC650B48, 0xE41D939C085B2390, 0x5AFB350579AF0B37,
                    0x32677CBE6E5C8982, 0xFC5F0DF24A56A255, 0x96B1E9441C59204F, 0x5C81154429F970F9,
                    0x1915912829DB9B40, 0x5A13D78FC1B83F23, 0x6EEE52535D11D777, 0xD369006D9516C770,
                    0x2EA97320423649D2, 0x3563C76014C98A0B, 0x856932A16FCB8046, 0x139E799BF19ACB97
                };

                public static readonly ulong[] TORRE =
                {
                    0xE619CEF3F77BC173, 0x33BBF206292C2429, 0x8E9E312CFE8C5881, 0x9F459FD766F66CD0,
                    0x4200024341044084, 0x751EADD75D897E3A, 0xB81A60A95FB27A01, 0xCF7EB441D6B6216F,
                    0x45F87717D361AE3F, 0xDF8AE1EACCDF5BC6, 0x3831065B7EBEAF8C, 0x7610DA4CD45BF178,
                    0xB3CA01C8647232A9, 0x3B8A09BBD9692680, 0xA4B7FC4588311CE7, 0x227B6B0525D6FBA6,
                    0xFCD846AF3F9143CB, 0xC0668E59E929F5EB, 0xCD4BBBA67A8ECE2B, 0x4CF18469C69FF8AD,
                    0xB85A9CAD9BEADC22, 0x368AA62A5E156F3E, 0x3D9C8CC41E7F8255, 0x51BC4DD6077180C2,
                    0xC6DCF18246EE4E54, 0xFE86FC3933DC618B, 0x8442E90A88144522, 0x5F851A7A44A41759,
                    0xD38924ED94DD6A26, 0x73401018CD54C21C, 0x829B8C77E325AC1A, 0xB307E2E54D26412E,
                    0xFBB65FA6E980E6AC, 0x85C66F561B83ED1F, 0x522C4359ACD84AEB, 0xF2AEFE9CB415A887,
                    0xDD41FB208AF8C5C0, 0x4953F9762848CE83, 0xF41091595C3DB2F9, 0xB49376C566FD93B9,
                    0xE4309F1ED0B124B5, 0xDDF24F88EEA5D722, 0x26EDDEDA5C169234, 0xF1697ADE82AB31EA,
                    0xC8158A1B60702E4E, 0x204A8049CAAB6221, 0x17F009A597B4C52A, 0x4ED1F088C0AB0217,
                    0x41C4A97A0B8B21EA, 0x8D1A31D16B01EE95, 0x3B4E05E83CFCF05E, 0x339527603763D857,
                    0xE9C9568FBE372403, 0xCED2D9D4CE125169, 0xB0D68C7EF58974D9, 0x604589E3C6CCC55F,
                    0xF8281D74D2FB4BD7, 0x83DFE578B4D1C37D, 0xE1040393F8EAD3FF, 0xEEC994716BCAE446,
                    0xD5E6A2ACBC432265, 0x441601B0952B7010, 0x17199E8C3A0BD6A5, 0x734ECB4B9428D28B
                };

                public static readonly ulong[] DAMA =
                {
                    0x28DCAC985610869C, 0xE3C845317609E760, 0x04C7224351B10B62, 0x812418B46D33D655,
                    0x4A4BC5BDD1CDAAD0, 0xCB080CF07F001991, 0x63D6AA99A81E7023, 0xFDFC6C1606F1F6FB,
                    0x40C0DCDB0E3D1F6E, 0x0F8A4C36A596BDCC, 0x925F9E50BFD4DB02, 0xC619033B45FD8BC5,
                    0x5DBB77CC8D8F1EF2, 0xD7B7213FC23349A9, 0x5E042E3863767659, 0xCBDC369A3E0184F2,
                    0xF79F7E40DB928F16, 0xA55F8F6CB6A1DCD0, 0xBAB5ACAC18C9E412, 0x954331AA2ED072FB,
                    0x306C913E96056B4D, 0x4E9C3003B04144DC, 0xB980FA5EDAD9DC43, 0xF1C9D04FFE1E9017,
                    0x11BFD47969068C70, 0x83541A80EFCBFD81, 0x0BE2B579FD38DAF8, 0x2B33AA54E7DE5000,
                    0x9E9B1E76F1AC81B8, 0x05EA957960C150EF, 0xA8242807FA61B151, 0xDBBFC5F07F37B9F5,
                    0xE375F48298B0EEED, 0xD5967109AA717C76, 0x1B3FC2B38F3B0DCD, 0xDC423A613A239101,
                    0x1516C775174623DE, 0x60E01BD804A47D6C, 0x8CCD80D3E5B84FE6, 0xF4A71B95A1A60E7C,
                    0xD6B25646F8096112, 0xFEC7D3A639E76A4C, 0xF17742C8709C3ADF, 0x715CF7E5A0A732D0,
                    0xE7D48B764613FDBD, 0xD514E26FA1747C21, 0xD0759BA7FA5A5BDB, 0x382720DACB6DF07E,
                    0x62919A3D3C2BE2F6, 0x8A4BA91E7ABDAC23, 0xE893BF3B8919B72F, 0xFED88809D9B8B561,
                    0x3A7D7094A6191028, 0x29CC08C8B49709A0, 0x993CAD4DD9D610FA, 0x9E75E8FBAD7A9633,
                    0xB4ED430A46151CC8, 0x78982B83C9FF2C1F, 0x5C0FC745D4A2E4F4, 0x654069266032A04E,
                    0x9AFF2766B05CEB46, 0x4142DDC4FE8AE5C1, 0xC37C6D157C0C6184, 0x6227E6F5B0E595B5
                };

                public static readonly ulong[] REI =
                {
                    0xCDEB431FAEE40146, 0xD585F951246E99E8, 0x0AE72E74F999FA16, 0x6C2600E48CB99754,
                    0xDE1BC2A05F26B911, 0x9C0112BF7628DC1C, 0xCCD66D72096FE4AC, 0x40002DA64C2F0184,
                    0x1D8DFE5B9C1A4047, 0x089FCA892DCA3B2A, 0xEBA61F4B671196B3, 0x86F1CE5F1EFBFED3,
                    0xFDFA02B2A64097D8, 0x8628F2AB63E35C8F, 0x3144E89DBD441519, 0x00BFF3ED00831504,
                    0x43443FA95BDA8933, 0xCF73A9D82009CC16, 0x3278C1E13B0FAB9D, 0xC3B240416EF13052,
                    0x4AB4F56B733744BD, 0xDEC77D36250723BE, 0xEA2AC135CFE15F67, 0xBCF73CC9BEF670FA,
                    0x3F7962A189299D85, 0xD1E171B3A29F6FDF, 0x693C768270CCB0DA, 0x53ECCEE8061CBEF9,
                    0x5CEC2995F5981637, 0x9923C5E556FBF38E, 0xFF6356DE28EF00A7, 0xAAD4637D75C7E813,
                    0x37259129782F3350, 0x606BAD7989B5B13E, 0x498E645116F12C23, 0xAF6CFA80A0CA4C15,
                    0xEA53F6996F334691, 0xF80FD737AA815BA9, 0xCA522EDE99A834DB, 0x65F4ABAA05A15B5D,
                    0x895CD513AA73A7B7, 0xDC877E91BD7412EE, 0xC8DCDBE3CDD88766, 0x6114CD29D74FEB98,
                    0x1C44D81B4C5B0E6A, 0x833819C51EF311FD, 0xDAF390C41F18B57B, 0xC1335572DED626CB,
                    0xE6E05DBF0B228173, 0xCAEC658BC939B031, 0x16762DE64DD13744, 0x9DAD6112EF4E1896,
                    0x084BFD9D44238F33, 0xF6731F5E2585DB3C, 0x2EEC57012C5F87F2, 0xE27A559B86A39DB5,
                    0x4BAD59B8F348C351, 0x2FE4AF4192E06D49, 0xBB9995B9355A3193, 0x54B5AA9C14EFB9C8,
                    0x6ABB0E1E40A20DB4, 0x6B12522CAC87835E, 0xBA99D38CFCEE1C22, 0x089211A984711756
                };
            }
        }
    }
}