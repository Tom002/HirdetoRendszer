using FluentValidation;
using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Common.Enum;
using System;

namespace HirdetoRendszer.Bll.Validators.Hirdetes
{
    public class HirdetesHozzaadasDtoValidator : AbstractValidator<HirdetesHozzaadasDto>
    {
        private Func<HirdetesHozzaadasDto, int?, bool> IdotartamFeltetel = (hirdetesHozzadas, _) =>
        {
            return hirdetesHozzadas.ErvenyessegVegOra > hirdetesHozzadas.ErvenyessegKezdetOra
                || (hirdetesHozzadas.ErvenyessegVegOra == hirdetesHozzadas.ErvenyessegKezdetOra && hirdetesHozzadas.ErvenyessegVegPerc > hirdetesHozzadas.ErvenyessegKezdetPerc);
        };

        public HirdetesHozzaadasDtoValidator()
        {
            When(x => x.IdosavhozKotott, () =>
            {
                RuleFor(x => x.ErvenyessegKezdetOra).NotNull();
                RuleFor(x => x.ErvenyessegKezdetPerc).NotNull();
                RuleFor(x => x.ErvenyessegVegOra).NotNull();
                RuleFor(x => x.ErvenyessegVegPerc).NotNull();

                RuleFor(m => m.ErvenyessegVegOra).Must(IdotartamFeltetel);
                RuleFor(m => m.ErvenyessegVegPerc).Must(IdotartamFeltetel);
                RuleFor(m => m.ErvenyessegKezdetOra).Must(IdotartamFeltetel);
                RuleFor(m => m.ErvenyessegKezdetPerc).Must(IdotartamFeltetel);
            });

            When(x => x.ElofizetesTipus == ElofizetesTipus.Havi, () =>
            {
                RuleFor(x => x.HaviLimit).NotNull().GreaterThan(0);
            });

            When(x => x.ElofizetesTipus == ElofizetesTipus.Mennyisegi, () =>
            {
                RuleFor(x => x.VasaroltIdotartam).NotNull().GreaterThan(0);
            });

            When(x => x.MindenVonalra, () =>
            {
                RuleFor(x => x.VonalIdLista).Empty();
            });

            When(x => !x.MindenVonalra, () =>
            {
                RuleFor(x => x.VonalIdLista).NotEmpty();
            });
        }
    }
}
