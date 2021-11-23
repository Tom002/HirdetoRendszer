using FluentValidation;
using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Common.Enum;

namespace HirdetoRendszer.Bll.Validators.Hirdetes
{
    public class HirdetesHozzaadasDtoValidator : AbstractValidator<HirdetesHozzaadasDto>
    {
        public HirdetesHozzaadasDtoValidator()
        {

            When(x => x.IdosavhozKotott, () =>
            {
                RuleFor(x => x.ErvenyessegKezdetOra).NotNull();
                RuleFor(x => x.ErvenyessegKezdetPerc).NotNull();
                RuleFor(x => x.ErvenyessegVegOra).NotNull();
                RuleFor(x => x.ErvenyessegVegPerc).NotNull();

                RuleFor(m => m.ErvenyessegVegOra).Must((hirdetesHozzadas, ervenyessegVegOra) =>
                {
                    return ervenyessegVegOra > hirdetesHozzadas.ErvenyessegKezdetOra
                        || (ervenyessegVegOra == hirdetesHozzadas.ErvenyessegKezdetOra && hirdetesHozzadas.ErvenyessegVegPerc > hirdetesHozzadas.ErvenyessegKezdetPerc);
                });
            });

            When(x => x.ElofizetesTipus == ElofizetesTipus.Havi, () =>
            {
                RuleFor(x => x.HaviLimit).NotNull().GreaterThan(0);
            });

            When(x => x.ElofizetesTipus == ElofizetesTipus.Mennyisegi, () =>
            {
                RuleFor(x => x.VasaroltIdotartam).NotNull().GreaterThan(0);
            });
        }
    }
}
